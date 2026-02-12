namespace ManejoDePresupuestos.ViewModelPartials
{
    public class NavegacionPorAñosViewModel
    {
        public DateTime FechaReferencia { get; set; }
        public int AñoPosterior => FechaReferencia.AddYears(1).Year;
        public int AñoAnterior => FechaReferencia.AddYears(-1).Year;
        public string Accion { get; set; } = string.Empty;
    }
}
