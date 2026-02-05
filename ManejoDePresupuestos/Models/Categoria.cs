using System.ComponentModel.DataAnnotations;

namespace ManejoDePresupuestos.Models
{
    public class Categoria
    {
        public int Id { get; set; }
        [Required]
        [StringLength(50, ErrorMessage = "El {0} no puede tener más de 50 caracteres.")]
        public string Nombre { get; set; } = string.Empty;

        [Display(Name = "Tipo de Operación")]
        public TipoOperacion TipoOperacionId { get; set; }
        public int UsuarioId { get; set; }
    }
}


   