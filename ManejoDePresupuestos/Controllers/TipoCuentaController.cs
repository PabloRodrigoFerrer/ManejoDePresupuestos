using Dapper;
using ManejoDePresupuestos.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace ManejoDePresupuestos.Controllers
{
    public class TipoCuentaController : Controller
    {

        public IActionResult Agregar()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Agregar(TipoCuenta tipoCuenta)
        {
            if (!ModelState.IsValid)
                return View(tipoCuenta);

            return View();
        }
    }
}
