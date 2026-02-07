using ManejoDePresupuestos.Models;
using ManejoDePresupuestos.Servicios;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;

namespace ManejoDePresupuestos.Controllers
{
    public class CategoriaController(IRepositorioCategoria repositorioCategoria, IRepositorioUsuario repositorioUsuario) : Controller
    {
        private readonly IRepositorioCategoria _repositorioCategoria = repositorioCategoria;
        private readonly IRepositorioUsuario _repositorioUsuario = repositorioUsuario;

        public async Task<IActionResult> Index()
        {
            var usuarioId = await _repositorioUsuario.ObtenerUsuarioId();
            var model = await _repositorioCategoria.ObtenerCategorias(usuarioId);

            return View(model);
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

        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            var usuarioId = await _repositorioUsuario.ObtenerUsuarioId();
            var categoria = await _repositorioCategoria.ObtenerCategoriaPorId(id, usuarioId);

            if (categoria is null)
                return RedirectToAction(nameof(Index));

            return View(categoria);
        }

        [HttpPost]
        public async Task<IActionResult> Editar(Categoria categoria)
        {
            var usuarioId = await _repositorioUsuario.ObtenerUsuarioId();
            var categoriaDb = await _repositorioCategoria.ObtenerCategoriaPorId(categoria.Id, usuarioId);

            if (categoriaDb is null)
                return RedirectToAction("NoEncontrado", "Home");

            await _repositorioCategoria.Editar(categoria);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Borrar(int id)
        {
            var usuarioId = await _repositorioUsuario.ObtenerUsuarioId();
            var categoria = await _repositorioCategoria.ObtenerCategoriaPorId(id, usuarioId);

            if (categoria is null)
                return RedirectToAction("NoEncontrado", "Home");

            return View(categoria);
        }

        [HttpPost]
        public async Task <IActionResult> BorrarCategoria(int id)
        {
            var usuarioId = await _repositorioUsuario.ObtenerUsuarioId();
            var categoria = await _repositorioCategoria.ObtenerCategoriaPorId(id, usuarioId);

            if (categoria is null)
                return RedirectToAction("NoEncontrado", "Home");

            await _repositorioCategoria.Borrar(id);
            return RedirectToAction(nameof(Index));
        }


       
       
    }
}
