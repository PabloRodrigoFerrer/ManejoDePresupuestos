using AutoMapper;
using ManejoDePresupuestos.Models;
using ManejoDePresupuestos.Servicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ManejoDePresupuestos.Controllers
{
    public class TransaccionController(IRepositorioUsuario repositorioUsuario,
        IRepositorioTransaccion repositorioTransaccion,
        IRepositorioCuenta repositorioCuenta,
        IRepositorioCategoria repositorioCategoria, IMapper _mapper) : Controller
    {
        private readonly IRepositorioUsuario _repositorioUsuario = repositorioUsuario;
        private readonly IRepositorioTransaccion _repositorioTransaccion = repositorioTransaccion;
        private readonly IRepositorioCuenta _repositorioCuentas = repositorioCuenta;
        private readonly IRepositorioCategoria _repositorioCategoria = repositorioCategoria;

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Agregar()
        {
            int usuarioId = await _repositorioUsuario.ObtenerUsuarioId();
            var model = new TransaccionAgregarViewModel
            {
                Cuentas = await ObtenerCuentasParaSelect(usuarioId)
            };

            model.Categorias = await ObtenerCategoriasParaSelect(usuarioId, model.TipoOperacionId);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Agregar(TransaccionAgregarViewModel modelo)
        {
            int usuarioId = await _repositorioUsuario.ObtenerUsuarioId();
            
            if (!ModelState.IsValid)
                return View(modelo);
            
            //chequeo que la cuenta y categoria que el usuario manda sea valida
            var cuenta = await _repositorioCuentas.ObtenerCuentaPorId(modelo.CuentaId, usuarioId);
            var categoria = await _repositorioCategoria.ObtenerCategoriaPorId(modelo.CategoriaId, usuarioId);
            if (cuenta is null || categoria is null)
                return RedirectToAction("NoEncontrado", "Home");

            if (modelo.TipoOperacionId is TipoOperacion.Gasto)
                modelo.Monto *= -1;

            modelo.UsuarioId = usuarioId;
            
            await _repositorioTransaccion.Agregar(modelo);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Editar(int id)
        {
            var usuarioId = await _repositorioUsuario.ObtenerUsuarioId();
            var transaccion = 
                await _repositorioTransaccion.ObtenerPorIdConTipoOperacion(id, usuarioId);

            if (transaccion is null)
                return RedirectToAction("NoEncontrado", "Home");

            transaccion.MontoAnterior = transaccion.TipoOperacionId is TipoOperacion.Gasto
                ? transaccion.MontoAnterior = transaccion.Monto * -1
                : transaccion.MontoAnterior = transaccion.Monto;

            transaccion.CuentaAnteriorId = transaccion.CuentaId;

            transaccion.Cuentas = await ObtenerCuentasParaSelect(usuarioId);
            transaccion.Categorias = await ObtenerCategoriasParaSelect(usuarioId, transaccion.TipoOperacionId);
            return View(transaccion);
        }

        [HttpPost]
        public async Task<IActionResult> Editar(TransaccionEditarViewModel transaccion)
        {
            var usuarioId = await _repositorioUsuario.ObtenerUsuarioId();

            if (!ModelState.IsValid)
                return View(transaccion);

            // validar id de  cuentas y categorias
            var cuentas = await _repositorioCuentas.ObtenerCuentaPorId(transaccion.CuentaId, usuarioId);
            var categoria = 
                await _repositorioCategoria.ObtenerCategoriaPorId(transaccion.CategoriaId, usuarioId);

            if (cuentas is null || categoria is null)
                return RedirectToAction("NoEncontrado", "Home");

            transaccion.Monto = transaccion.TipoOperacionId is TipoOperacion.Gasto
                 ? transaccion.Monto *= -1
                 : transaccion.Monto;

            await _repositorioTransaccion
                .Editar(transaccion, transaccion.CuentaAnteriorId, transaccion.MontoAnterior);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Borrar(int id)
        {
            // validar que transacción pertenece al usuario que hace la solicitud
            var usuarioId = await _repositorioUsuario.ObtenerUsuarioId();
            var transaccion = await _repositorioTransaccion.ObtenerPorId(id, usuarioId);
            if (transaccion is null) return RedirectToAction("NoEncontrado", "Home");

            await _repositorioTransaccion.Borrar(id);
            return RedirectToAction("Index");
        }

        private async Task<IEnumerable<SelectListItem>> ObtenerCategoriasParaSelect(int usuarioId, TipoOperacion tipoOperacionid)
        {
            var categorias = await _repositorioCategoria.ObtenerPorTipoOperacion(usuarioId, tipoOperacionid);
            return categorias.Select(c => new SelectListItem(c.Nombre, c.Id.ToString()));
        }

        private async Task<IEnumerable<SelectListItem>> ObtenerCuentasParaSelect(int usuarioId)
        {
            var cuentas = await _repositorioCuentas.ObtenerCuentas(usuarioId);
            return cuentas.Select(c => new SelectListItem(c.Nombre, c.Id.ToString()));
        }

        [HttpPost] // Endpoint para actualizar el dropdown dependendiente a través de fetch
        public async Task<IActionResult> CategoriasPorTipoOperacion([FromBody] TipoOperacion idTipoOperacion)
        {
            var usuarioId = await _repositorioUsuario.ObtenerUsuarioId();
            var categorias =await ObtenerCategoriasParaSelect(usuarioId, idTipoOperacion);

            return Ok(categorias);
        }
    }
}
