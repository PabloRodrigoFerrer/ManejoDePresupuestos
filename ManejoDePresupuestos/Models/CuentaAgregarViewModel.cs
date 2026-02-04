using Microsoft.AspNetCore.Mvc.Rendering;

namespace ManejoDePresupuestos.Models
{
    public class CuentaAgregarViewModel : Cuenta
    {
        public IEnumerable<SelectListItem> TiposCuentas { get; set; } = [];
    }
}
