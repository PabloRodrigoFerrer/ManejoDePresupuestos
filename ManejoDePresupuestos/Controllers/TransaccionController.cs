using AutoMapper;
using ManejoDePresupuestos.Models;
using ManejoDePresupuestos.Servicios;
using ManejoDePresupuestos.ViewModelPartials;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;

namespace ManejoDePresupuestos.Controllers
{
    public class TransaccionController(IRepositorioUsuario repositorioUsuario,
        IRepositorioTransaccion repositorioTransaccion,
        IRepositorioCuenta repositorioCuenta,
        IRepositorioCategoria repositorioCategoria, IMapper _mapper) : Controller
    {
        private readonly IRepositorioUsuario _repositorioUsuario = repositorioUsuario;
        private readonly IRepositorioTransaccion _repositorioTransaccion = repositorioTransaccion;
        private readonly IRepositorioCuenta _repositorioCuentas = repositorioCuenta;
        private readonly IRepositorioCategoria _repositorioCategoria = repositorioCategoria;

        //GET 
        public async Task<IActionResult> Index(int mes, int año)
        {
            int usuarioId = await _repositorioUsuario.ObtenerUsuarioId();

            var model = await GenerarVmTransaccionesPorUsuario(usuarioId, mes, año);
            
            return View(model);
        }

        //GET
        public async Task<IActionResult> Agregar()
        {
            int usuarioId = await _repositorioUsuario.ObtenerUsuarioId();
            var model = new TransaccionAgregarViewModel
            {
                Cuentas = await ObtenerCuentasParaSelect(usuarioId)
            };
            model.Categorias = await ObtenerCategoriasParaSelect(usuarioId, model.TipoOperacionId);
          
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Agregar(TransaccionAgregarViewModel modelo)
        {
            int usuarioId = await _repositorioUsuario.ObtenerUsuarioId();

            if (!ModelState.IsValid)
                return View(modelo);

            //chequeo que la cuenta y categoria que el usuario manda sea valida
            var cuenta = await _repositorioCuentas.ObtenerCuentaPorId(modelo.CuentaId, usuarioId);
            var categoria = await _repositorioCategoria.ObtenerCategoriaPorId(modelo.CategoriaId, usuarioId);
            if (cuenta is null || categoria is null)
                return RedirectToAction("NoEncontrado", "Home");

            if (modelo.TipoOperacionId is TipoOperacion.Gasto)
                modelo.Monto *= -1;

            modelo.UsuarioId = usuarioId;

            await _repositorioTransaccion.Agregar(modelo);
            
            return RedirectToAction("Index");
        }

        //GET
        public async Task<IActionResult> Editar(int id, string? urlRetorno = null)
        {
            var usuarioId = await _repositorioUsuario.ObtenerUsuarioId();
            var transaccion =
                await _repositorioTransaccion.ObtenerPorIdConTipoOperacion(id, usuarioId);

            if (transaccion is null)
                return RedirectToAction("NoEncontrado", "Home");

            transaccion.MontoAnterior = transaccion.TipoOperacionId is TipoOperacion.Gasto
                ? transaccion.MontoAnterior = transaccion.Monto * -1
                : transaccion.MontoAnterior = transaccion.Monto;

            transaccion.CuentaAnteriorId = transaccion.CuentaId;

            transaccion.Cuentas = await ObtenerCuentasParaSelect(usuarioId);
            transaccion.Categorias = await ObtenerCategoriasParaSelect(usuarioId, transaccion.TipoOperacionId);

            if (urlRetorno is not null)
                transaccion.UrlRetorno = urlRetorno;

            return View(transaccion);
        }

        [HttpPost]
        public async Task<IActionResult> Editar(TransaccionEditarViewModel transaccion)
        {
            var usuarioId = await _repositorioUsuario.ObtenerUsuarioId();

            if (!ModelState.IsValid)
                return View(transaccion);

            // validar id de  cuentas y categorias
            var cuenta = await _repositorioCuentas.ObtenerCuentaPorId(transaccion.CuentaId, usuarioId);
            var categoria =
                await _repositorioCategoria.ObtenerCategoriaPorId(transaccion.CategoriaId, usuarioId);

            if (cuenta is null || categoria is null)
                return RedirectToAction("NoEncontrado", "Home");

            transaccion.Monto = transaccion.TipoOperacionId is TipoOperacion.Gasto
                 ? transaccion.Monto *= -1
                 : transaccion.Monto;

            await _repositorioTransaccion
                .Editar(transaccion, transaccion.CuentaAnteriorId, transaccion.MontoAnterior);

            if (string.IsNullOrEmpty(transaccion.UrlRetorno))
                return RedirectToAction("Index");
            else
                return LocalRedirect(transaccion.UrlRetorno);         
        }

        [HttpPost]
        public async Task<IActionResult> Borrar(int id, string? urlRetorno = null)
        {
            // validar que transacción pertenece al usuario que hace la solicitud
            var usuarioId = await _repositorioUsuario.ObtenerUsuarioId();
            var transaccion = await _repositorioTransaccion.ObtenerPorId(id, usuarioId);
            if (transaccion is null) return RedirectToAction("NoEncontrado", "Home");

            await _repositorioTransaccion.Borrar(id);

            if (urlRetorno is null)
                return RedirectToAction("Index");
            else
                return LocalRedirect(urlRetorno);
        }

        [HttpGet]
        public async Task<IActionResult> TransaccionesDetallePorCuenta(int cuentaId, int mes, int año)
        {
            int usuarioId = await _repositorioUsuario.ObtenerUsuarioId();
            var cuenta = await _repositorioCuentas.ObtenerCuentaPorId(cuentaId, usuarioId);

            if (cuenta is null) return RedirectToAction("NoEncontrado", "Home");

            var model = await GenerarVmTransaccionesPorCuenta(cuentaId, usuarioId, mes, año);

            ViewBag.Cuenta = cuenta.Nombre;

            return View(model);
        }

        [HttpPost] // Endpoint para actualizar el dropdown dependendiente a través de fetch
        public async Task<IActionResult> CategoriasPorTipoOperacion([FromBody] TipoOperacion idTipoOperacion)
        {
            var usuarioId = await _repositorioUsuario.ObtenerUsuarioId();
            var categorias = await ObtenerCategoriasParaSelect(usuarioId, idTipoOperacion);

            return Ok(categorias);
        }
        //GET
        public async Task<IActionResult> Diario()
        {
            return View();
        }

        //GET
        public async Task<IActionResult> Semanal(int mes, int año)
        {
            var usuarioId = await _repositorioUsuario.ObtenerUsuarioId();

            (DateTime fechaInicio, DateTime fechaFin) = ObtenerFechaInicioFin(mes, año);

            var TransaccionesSemanales = (List<ReporteSemanalDTO>)
                await _repositorioTransaccion.ObtenerTransaccionesSemanales(usuarioId, fechaInicio, fechaFin);
     
            //algorimo para calcular dias de la semana y cantidad de semanas
            var diasDelMes = Enumerable.Range(0, fechaFin.Day);
            var semanasDelMes = diasDelMes.Chunk(7).ToList();

            for (var i = 0; i < semanasDelMes.Count; i++)
            {
                var semanaIteracion = i + 1;
                var diaInicio = semanasDelMes[i].First() + 1;
                var diaFinal = semanasDelMes[i].Last() + 1;

                var semana = TransaccionesSemanales.Where(r => r.Semana == semanaIteracion).FirstOrDefault();
                if(semana is null)
                {
                    TransaccionesSemanales.Add(new ReporteSemanalDTO
                    {
                        Semana = semanaIteracion,
                        FechaInicioSemana = new DateTime(fechaInicio.Year, fechaInicio.Month, diaInicio),
                        FechaFinalSemana = new DateTime(fechaFin.Year, fechaFin.Month, diaFinal),
                    });
                }
                else
                {
                    semana.FechaInicioSemana = new DateTime(fechaInicio.Year, fechaInicio.Month, diaInicio);
                    semana.FechaFinalSemana = new DateTime(fechaFin.Year, fechaFin.Month, diaFinal);
                }
            }

            var model = new ReporteSemanalViewModel
            {
                ReportesSemanales = TransaccionesSemanales.OrderByDescending(f => f.FechaInicioSemana),
                Navegacion = new NavegacionFechasViewModel { FechaReferencia = fechaInicio, Accion = nameof(Semanal) }
            };

            return View(model);
        }

        //GET
        public async Task<IActionResult> Mensual()
        {
            return View();
        }


        //GET
        public async Task<IActionResult> Excel()
        {
            return View();
        }

        //GET
        public async Task<IActionResult> Calendario()
        {
            return View();
        }

        private async Task<IEnumerable<SelectListItem>> ObtenerCategoriasParaSelect(int usuarioId, TipoOperacion tipoOperacionid)
        {
            var categorias = await _repositorioCategoria.ObtenerPorTipoOperacion(usuarioId, tipoOperacionid);
            return categorias.Select(c => new SelectListItem(c.Nombre, c.Id.ToString()));
        }

        private async Task<IEnumerable<SelectListItem>> ObtenerCuentasParaSelect(int usuarioId)
        {
            var cuentas = await _repositorioCuentas.ObtenerCuentas(usuarioId);
            return cuentas.Select(c => new SelectListItem(c.Nombre, c.Id.ToString()));
        }

        private async Task<TransaccionesPorCuentaViewModel> GenerarVmTransaccionesPorCuenta(int cuentaId, int usuarioId, int mes, int año)
        {
            var model = new TransaccionesPorCuentaViewModel { CuentaId = cuentaId };
            (model.FechaInicio, model.FechaFin) = ObtenerFechaInicioFin(mes, año);

            var transacciones = 
                await _repositorioTransaccion.ObtenerTransaccionesPorCuenta(cuentaId, usuarioId, model.FechaInicio, model.FechaFin);
            
            GenerarTransaccionesPorFecha(model, transacciones);
            model.UrlRetorno = HttpContext.Request.Path + HttpContext.Request.QueryString;
            model.Navegacion = new NavegacionFechasViewModel
            {
                Accion = nameof(TransaccionesDetallePorCuenta),
                FechaReferencia = model.FechaInicio,
                Parametro = cuentaId
            };

            return model;
        }

        private async Task<TransaccionesPorCuentaViewModel> GenerarVmTransaccionesPorUsuario(int usuarioId, int mes, int año)
        {
            var model = new TransaccionesPorCuentaViewModel() ;
            (model.FechaInicio, model.FechaFin) = ObtenerFechaInicioFin(mes, año);

            var transacciones =
                await _repositorioTransaccion.ObtenerTransaccionesPorUsuario(usuarioId, model.FechaInicio, model.FechaFin);
            
            GenerarTransaccionesPorFecha(model, transacciones);
            model.UrlRetorno = HttpContext.Request.Path + HttpContext.Request.QueryString;

            model.Navegacion = new NavegacionFechasViewModel
            {
                Accion = nameof(Index),
                FechaReferencia = model.FechaInicio
            };

            return model;
        }

        //procedimiento pasando instancia de vm
        private void GenerarTransaccionesPorFecha(TransaccionesPorCuentaViewModel model, IEnumerable<TransaccionDetalleDTO> transacciones)
        {
            model.TransaccionesAgrupadas =
                    transacciones
                    .OrderByDescending(t => t.FechaTransaccion)
                    .GroupBy(t => t.FechaTransaccion)
                    .Select(grupo => new TransaccionesPorFecha
                    {
                        FechaTransaccion = grupo.Key,
                        Transacciones = grupo.AsEnumerable()
                    });
        }

        //obtener por funcion sin instancia de vm
        private IEnumerable<TransaccionesPorFecha> GenerarTransaccionesPorFecha(IEnumerable<TransaccionDetalleDTO> transacciones)
        {
            var TransaccionesAgrupadas =
                    transacciones
                    .OrderByDescending(t => t.FechaTransaccion)
                    .GroupBy(t => t.FechaTransaccion)
                    .Select(grupo => new TransaccionesPorFecha
                    {
                        FechaTransaccion = grupo.Key,
                        Transacciones = grupo.AsEnumerable()
                    });

           return TransaccionesAgrupadas;
        }

        private (DateTime fechaInicio, DateTime fechaFin) ObtenerFechaInicioFin(int mes, int año)
        {
           var fechaInicio = DateTime.Now;
            
           fechaInicio = mes <= 0 || mes > 12 || año < 1900
           ? fechaInicio = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1)
           : fechaInicio = new DateTime(año, mes, 1);

            var fechaFin = fechaInicio.AddMonths(1).AddDays(-1);

            return (fechaInicio, fechaFin);
        }
    }
}
