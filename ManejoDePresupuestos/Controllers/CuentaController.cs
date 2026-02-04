using ManejoDePresupuestos.Models;
using ManejoDePresupuestos.Servicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ManejoDePresupuestos.Controllers
{
    public class CuentaController : Controller
    {
        private readonly IRepositorioTipoCuenta _respositorioTipoCuenta;
        private readonly IRepositorioUsuario _repositorioUsuario;
        private readonly IRepositorioCuenta _repositorioCuenta;

        public CuentaController(
            IRepositorioTipoCuenta repositorioTipoCuenta,
            IRepositorioUsuario repositorioUsuario,
            IRepositorioCuenta repositorioCuenta)
        {
            _respositorioTipoCuenta = repositorioTipoCuenta;
            _repositorioUsuario = repositorioUsuario;
            _repositorioCuenta = repositorioCuenta;
        }


        public async Task<IActionResult> Index()
        {
            //manejar implementación para agregar y listar cuentas.. 
            int usuarioId = await _repositorioUsuario.ObtenerUsuarioId();
            var cuentas = await _repositorioCuenta.ObtenerCuentas(usuarioId);

            var model = cuentas.GroupBy(c => c.TipoCuenta)
                .Select(grupo => new IndexCuentaViewModel
                {
                    TipoCuenta = grupo.Key,
                    Cuentas = grupo.AsEnumerable()
                }).ToList();

            return View(model);
        }

        public async Task<IActionResult> Agregar()
        {
            var usuarioId = await _repositorioUsuario.ObtenerUsuarioId();
            var tipoCuentas = await _respositorioTipoCuenta.ObtenerCuentasPorUsuario(usuarioId);

            var model = new CuentaViewModel();
            model.TiposCuentas = tipoCuentas.Select(c =>
                new SelectListItem(c.Nombre, c.Id.ToString()));
        
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Agregar(CuentaViewModel cuenta)
        {
            //validamos que tipo cuenta pertenezca al usuario
            int usuarioId = await _repositorioUsuario.ObtenerUsuarioId();
            var tipoCuenta = await _respositorioTipoCuenta.ObtenerPorId(cuenta.TipoCuentaId, usuarioId);

            if (tipoCuenta is null)
                return RedirectToAction("NoEncontrado", "Home");
            
            if (!ModelState.IsValid)
            {
                return View(cuenta);
            }

            await _repositorioCuenta.Create(cuenta);
            return RedirectToAction("Index");
        }
    }
}
