

namespace ManejoDePresupuestos.ViewModelPartials
{
    public class NavegacionFechasViewModel
    {
        public DateTime FechaReferencia { get; set; }
        public int MesAnterior => FechaReferencia.AddMonths(-1).Month;
        public int AñoAnterior => FechaReferencia.AddMonths(-1).Year;
        public int MesPosterior => FechaReferencia.AddMonths(1).Month;
        public int AñoPosterior => FechaReferencia.AddMonths(1).Year;
        public string Accion {  get; set; } = string.Empty;
        public int? Parametro { get; set; }
    }
}
