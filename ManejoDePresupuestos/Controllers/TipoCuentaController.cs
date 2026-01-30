using Dapper;
using ManejoDePresupuestos.Models;
using ManejoDePresupuestos.Servicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace ManejoDePresupuestos.Controllers
{
    public class TipoCuentaController : Controller
    {
        private readonly IRepositorioTipoCuenta _repositorioTipoCuenta;

        public TipoCuentaController(IRepositorioTipoCuenta repositorioTipoCuenta)
        {
            _repositorioTipoCuenta = repositorioTipoCuenta;
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

            tipoCuenta.UsuarioId = 1; //Temporal hasta tener implementado el sistema de usuarios
            await _repositorioTipoCuenta.Create(tipoCuenta);

            return View();
        }
    }
}
