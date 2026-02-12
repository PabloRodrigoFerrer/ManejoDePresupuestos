using Dapper;
using ManejoDePresupuestos.Validaciones;
using Microsoft.Data.SqlClient;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ManejoDePresupuestos.Models
{
    public class Cuenta
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El {0} es obligatorio")]
        [PrimeraLetraMayuscula]
        [StringLength(50, ErrorMessage = "El {0} no puede tener más de 50 caracteres.")]
        public string Nombre { get; set; } = string.Empty;

        [DisplayName("Tipo cuenta")]
        public int TipoCuentaId { get; set; }
        public string TipoCuenta { get; set; } = string.Empty;
        public decimal Balance { get; set; }

        [StringLength(1000, ErrorMessage = "El {0} no puede contener más de 1000 caracteres.")]
        public string Descripcion { get; set; } = string.Empty;

    }
}
