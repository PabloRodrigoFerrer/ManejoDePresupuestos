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

        public CuentaController(IRepositorioTipoCuenta repositorioTipoCuenta, IRepositorioUsuario repositorioUsuario)
        {
            _respositorioTipoCuenta = repositorioTipoCuenta;
            _repositorioUsuario = repositorioUsuario;
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

    }
}
