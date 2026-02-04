using ManejoDePresupuestos.Models;
using ManejoDePresupuestos.Servicios;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ManejoDePresupuestos.Controllers
{
    public class CategoriaController : Controller
    {
        private readonly IRepositorioCategoria _repositorioCategoria;
        private readonly IRepositorioUsuario _repositorioUsuario;

        public CategoriaController(IRepositorioCategoria repositorioCategoria, IRepositorioUsuario repositorioUsuario)
        {
           _repositorioCategoria = repositorioCategoria;
           _repositorioUsuario = repositorioUsuario;
        }

        public IActionResult Index()
        {
            return View();
        }
            

        public async Task<IActionResult> Agregar()
        {

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Agregar(Categoria categoria)
        {
            if(!ModelState.IsValid)
                RedirectToAction(nameof(Index));

            int usuarioId = await _repositorioUsuario.ObtenerUsuarioId();
            categoria.UsuarioId = usuarioId;
            await _repositorioCategoria.Agregar(categoria);
            return RedirectToAction(nameof(Index));
        }

    }
}
