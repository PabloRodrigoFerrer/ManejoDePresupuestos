using Microsoft.AspNetCore.Mvc.Rendering;

namespace ManejoDePresupuestos.Models
{
    public class TransaccionAgregarViewModel : Transaccion
    {
        public IEnumerable<SelectListItem> Cuentas { get; set; } = [];
        public IEnumerable<SelectListItem> Categorias { get; set; } = [];

        public TipoOperacion TipoOperacion { get; set; } = TipoOperacion.Ingreso;
    }
}
