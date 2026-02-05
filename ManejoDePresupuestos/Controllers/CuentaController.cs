using AutoMapper;
using ManejoDePresupuestos.Models;
using ManejoDePresupuestos.Servicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ManejoDePresupuestos.Controllers
{
    public class CuentaController(
        IRepositorioTipoCuenta repositorioTipoCuenta,
        IRepositorioUsuario repositorioUsuario,
        IRepositorioCuenta repositorioCuenta,
        IMapper mapper) : Controller
    {
        private readonly IRepositorioTipoCuenta _respositorioTipoCuenta = repositorioTipoCuenta;
        private readonly IRepositorioUsuario _repositorioUsuario = repositorioUsuario;
        private readonly IRepositorioCuenta _repositorioCuenta = repositorioCuenta;
        private readonly IMapper _mapper = mapper;

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

            var model = new CuentaAgregarViewModel
            {
                TiposCuentas = tipoCuentas.Select(c =>
                    new SelectListItem(c.Nombre, c.Id.ToString()))
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Agregar(CuentaAgregarViewModel cuenta)
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


        public async Task<IActionResult> Editar(int id)
        { 
            int usuarioId = await _repositorioUsuario.ObtenerUsuarioId();
            var cuenta = await _repositorioCuenta.ObtenerCuentaPorId(id, usuarioId);

            if (cuenta is null)
                return RedirectToAction("NoEncontrado", "Home");

            var viewModel = _mapper.Map<CuentaAgregarViewModel>(cuenta);

            var tipoCuentas = await _respositorioTipoCuenta.ObtenerCuentasPorUsuario(usuarioId);
            viewModel.TiposCuentas = tipoCuentas.Select(tc =>
                    new SelectListItem(tc.Nombre, tc.Id.ToString()));

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Editar(Cuenta cuentaEditar)
        {
            int usuarioId = await _repositorioUsuario.ObtenerUsuarioId();
            var cuenta = await _repositorioCuenta.ObtenerCuentaPorId(cuentaEditar.Id, usuarioId);

            if (cuenta is null)
                return RedirectToAction("NoEncontrado", "Home");

            await _repositorioCuenta.Editar(cuentaEditar);
            return RedirectToAction("Index");

        }

        [HttpGet]
        public async Task<IActionResult> Borrar(int id)
        {
            int usuarioId = await _repositorioUsuario.ObtenerUsuarioId();
            var cuenta = await _repositorioCuenta.ObtenerCuentaPorId(id, usuarioId);

            if (cuenta is null || cuenta.Id != id)
                return RedirectToAction("NoEncontrado", "Home");

            return View(cuenta);
        }

        [HttpPost]
        public async Task<IActionResult> BorrarCuenta(int id) 
        {
            int usuarioId = await _repositorioUsuario.ObtenerUsuarioId();
            var cuenta = await _repositorioCuenta.ObtenerCuentaPorId(id, usuarioId);

            if (cuenta is null || cuenta.Id != id)
                return RedirectToAction("NoEncontrado", "Home");

            await _repositorioCuenta.Borrar(id);
            return RedirectToAction("Index");
        }

       
    }
}
