using Microsoft.AspNetCore.Mvc.Rendering;

namespace ManejoDePresupuestos.Models
{
    public class TransaccionAgregarViewModel : Transaccion
    {
        public IEnumerable<SelectListItem> Cuentas = [];
        public IEnumerable<SelectListItem> Categorias = [];
    }
}
