namespace ManejoDePresupuestos.Models
{
    public class TransaccionEditarViewModel : TransaccionAgregarViewModel
    {
        public decimal MontoAnterior { get; set; }
        public int cuentaAnteriorId { get; set; }
    }
}
