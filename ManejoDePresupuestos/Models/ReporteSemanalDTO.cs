namespace ManejoDePresupuestos.Models
{
    public class ReporteSemanalDTO
    {
        public int Semana { get; set;  }
        public DateTime FechaInicioSemana { get; set; }
        public DateTime FechaFinalSemana {  get; set; }

        public decimal Ingresos { get; set; }
        public decimal Gastos { get;set;  }
    }
}
