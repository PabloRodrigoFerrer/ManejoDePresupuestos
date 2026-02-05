using ManejoDePresupuestos.Models;
using ManejoDePresupuestos.Servicios;
using Microsoft.AspNetCore.Mvc;

namespace ManejoDePresupuestos.Controllers
{
    public class TipoCuentaController : Controller
    {
        private readonly IRepositorioTipoCuenta _repositorioTipoCuenta;
        private readonly IRepositorioUsuario _repositorioUsuario;

        public TipoCuentaController(
            IRepositorioTipoCuenta repositorioTipoCuenta,
            IRepositorioUsuario repositorioUsuario
            )
        {
            _repositorioTipoCuenta = repositorioTipoCuenta;
            _repositorioUsuario = repositorioUsuario;
        }

        public async Task<IActionResult> Index()
        {
            var usuarioId = await _repositorioUsuario.ObtenerUsuarioId();
            var listaCuentas = await _repositorioTipoCuenta.ObtenerCuentasPorUsuario(usuarioId);

            return View(listaCuentas);
        }

        public IActionResult Agregar()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Agregar(TipoCuenta tipoCuenta)
        {
            if (!ModelState.IsValid)
                return View(tipoCuenta);

            tipoCuenta.UsuarioId = await _repositorioUsuario.ObtenerUsuarioId(); //Temporal hasta tener implementado el sistema de usuarios
            bool existe = await _repositorioTipoCuenta.Existe(tipoCuenta.Nombre, tipoCuenta.UsuarioId);

            if (existe)
            {
                ModelState.AddModelError(nameof(tipoCuenta.Nombre), $"El nombre {tipoCuenta.Nombre} ya existe.");
                return View(tipoCuenta);
            }

            await _repositorioTipoCuenta.Create(tipoCuenta);
            return RedirectToAction("Index");
        }


        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            int usuarioId = await _repositorioUsuario.ObtenerUsuarioId();
            var tipoCuenta = await _repositorioTipoCuenta.ObtenerPorId(id, usuarioId);

            if (tipoCuenta is null)
                return RedirectToAction("NoEncontrado", "Home");

            return View(tipoCuenta);
        }

        [HttpPost]
        public async Task<IActionResult> Editar(TipoCuenta tipoCuenta)
        {
            if (!ModelState.IsValid)
                return View(tipoCuenta);
            await _repositorioTipoCuenta.Editar(tipoCuenta);
            return RedirectToAction("index");
        }

        [HttpGet]
        public async Task<IActionResult> Borrar(int id)
        {
            int usuarioId = await _repositorioUsuario.ObtenerUsuarioId();
            var tipoCuenta = await _repositorioTipoCuenta.ObtenerPorId(id, usuarioId);

            if (tipoCuenta is null)
                return RedirectToAction("NoEncontrado", "Home");

            return View(tipoCuenta);
        }

        [HttpPost]
        public async Task<IActionResult> BorrarCuenta(int id)
        {
            int usuarioId = await _repositorioUsuario.ObtenerUsuarioId();
            var tipoCuenta = await _repositorioTipoCuenta.ObtenerPorId(id, usuarioId);

            if (tipoCuenta is null)
                return RedirectToAction("NoEncontrado", "Home");

            await _repositorioTipoCuenta.Borrar(id);

            return RedirectToAction("index");
        }

        [HttpGet]
        public async Task<IActionResult> VerificarExisteTipoCuenta(string nombre)
        {
            int id = await _repositorioUsuario.ObtenerUsuarioId();
            var existe = await _repositorioTipoCuenta.Existe(nombre, id);

            if (existe)
                return Json($"El nombre {nombre} de esa cuenta ya existe");

            return Json(true);
        }

        [HttpPost]
        public async Task<IActionResult> Ordenar([FromBody] int[] ids)
        {
            int usuarioId = await _repositorioUsuario.ObtenerUsuarioId();
            var tiposCuentas = await _repositorioTipoCuenta.ObtenerCuentasPorUsuario(usuarioId);

            int[] idsCuentasUsuario = tiposCuentas.Select(x => x.Id).ToArray();
            //comparación ids cuentas con ids del usuario
            var idsNoPertenecenAlUsuario = ids.Except(idsCuentasUsuario).ToArray();

            if(idsNoPertenecenAlUsuario.Length > 0)
                return Forbid();


            var tiposCuentasOrdenados =
                ids.Select((valor, indice) => new TipoCuenta
                {
                    Id = valor,
                    Orden = indice + 1,
                }).ToArray();

            await _repositorioTipoCuenta.Ordenar(tiposCuentasOrdenados);

            return Ok();
        }
    }
}
