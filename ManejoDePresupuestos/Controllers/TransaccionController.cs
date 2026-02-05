using ManejoDePresupuestos.Models;
using ManejoDePresupuestos.Servicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ManejoDePresupuestos.Controllers
{
    public class TransaccionController(IRepositorioUsuario repositorioUsuario,
        IRepositorioTransaccion repositorioTransaccion,
        IRepositorioCuenta repositorioCuenta,
        IRepositorioCategoria repositorioCategoria) : Controller
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
            var cuentas = await _repositorioCuentas.ObtenerCuentas(usuarioId);
            var categorias = await _repositorioCategoria.ObtenerCategorias(usuarioId);

            TransaccionAgregarViewModel model = new()
            {
                Cuentas = cuentas.Select(c =>
                    new SelectListItem(c.Nombre, c.Id.ToString())),

                Categorias = categorias.Select(c =>
                        new SelectListItem(c.Nombre, c.Id.ToString()))
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Agregar(Transaccion transaccion)
        {
            int usuarioId = await _repositorioUsuario.ObtenerUsuarioId();
            transaccion.UsuarioId = usuarioId;
            
            if (!ModelState.IsValid)
            {
                return View(transaccion);
            }

            await _repositorioTransaccion.Agregar(transaccion);
            return RedirectToAction("Index");

        }
    }
}
