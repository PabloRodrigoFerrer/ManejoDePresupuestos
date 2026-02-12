using Microsoft.VisualBasic;

namespace ManejoDePresupuestos.Models
{
    public class ReporteMensualDTO
    {
        public int Mes { get; set; }
        public int Año { get; set; }
        public decimal Ingresos { get; set; }
        public decimal Gastos { get; set; }

        public DateTime FechaReferencia => new DateTime(Año, Mes, 1);
    }
}
