using ManejoDePresupuestos.Validaciones;
using Microsoft.AspNetCore.Antiforgery;
using System.ComponentModel.DataAnnotations;

namespace ManejoDePresupuestos.Models
{
    public class TipoCuenta
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El {0} es obligatorio")]
        [PrimeraLetraMayuscula]
        public string Nombre { get; set; } = string.Empty;

        public int UsuarioId { get; set; }

        public int Orden { get; set; }
    }
}
