namespace ManejoDePresupuestos.Models
{
    public class Categoria
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        
        public TipoOperacion TipoOperacionId { get; set; }
        public int UsuarioId { get; set; }

    }
}


   