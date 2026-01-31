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

        public async Task<IActionResult> Index()
        {
            var usuarioId = 1;
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

            tipoCuenta.UsuarioId = 1; //Temporal hasta tener implementado el sistema de usuarios
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
        public async Task<IActionResult> VerificarExisteTipoCuenta(string nombre)
        {
            int id = 1;
            var existe = await _repositorioTipoCuenta.Existe(nombre, id);
            
            if (existe)
                return Json($"El nombre {nombre} de esa cuenta ya existe");
            
            return Json(true);
        }

        

    }
}
