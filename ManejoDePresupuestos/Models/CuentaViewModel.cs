using Microsoft.AspNetCore.Mvc.Rendering;

namespace ManejoDePresupuestos.Models
{
    public class CuentaViewModel : Cuenta
    {
        public IEnumerable<SelectListItem> TiposCuentas { get; set; } = [];
    }
}
