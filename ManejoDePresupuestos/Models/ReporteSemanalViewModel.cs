
using ManejoDePresupuestos.ViewModelPartials;
using System.Reflection;

namespace ManejoDePresupuestos.Models
{
    public class ReporteSemanalViewModel
    {
        public IEnumerable<ReporteSemanalDTO> ReportesSemanales { get; set; } = [];
        public decimal TotalIngresosSemanales => ReportesSemanales.Sum(s => s.Ingresos);
        public decimal TotalGastosSemanales => ReportesSemanales.Sum(s => s.Gastos);
        public decimal TotalBalance => TotalIngresosSemanales - TotalGastosSemanales;
        
        public BalanceTotalesViewModel Totales => new BalanceTotalesViewModel
        {
            Ingresos = TotalIngresosSemanales,
            Gastos = TotalGastosSemanales,
            Total = TotalBalance
        };

        public NavegacionFechasViewModel? Navegacion {get; set;} 
       
    }
}
