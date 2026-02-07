using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ManejoDePresupuestos.Models
{
    public class Transaccion
    {
        public int Id { get; set; }

        [Required]
        public int UsuarioId { get; set; }

        [Display(Name ="Fecha transacción")]
        [DataType(DataType.DateTime)]
        public DateTime FechaTransaccion { get; set; } = DateTime.Parse(DateTime.Now.ToString("g"));

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "El monto debe ser mayor que cero.")]
        public decimal Monto { get; set; }

        [MaxLength(1000, ErrorMessage = "La nota no puede contener más de {1} caracteres")]
        public string? Nota { get; set; } = string.Empty;

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Id de cuenta invalido")]
        [Display(Name = "Cuenta")]
        public int CuentaId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Id de cuenta invalido")]
        [Display(Name ="Categoría")]
        public int CategoriaId { get; set; }
    }
}
