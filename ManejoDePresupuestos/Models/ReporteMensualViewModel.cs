using ManejoDePresupuestos.ViewModelPartials;

namespace ManejoDePresupuestos.Models
{
    public class ReporteMensualViewModel
    {
        public IEnumerable<ReporteMensualDTO> Reportes { get; set; } = [];
        public NavegacionPorAñosViewModel? Navegacion { get; set; }
        public BalanceTotalesViewModel Totales => new BalanceTotalesViewModel
        {
            Ingresos = Reportes.Sum(r => r.Ingresos),
            Gastos = Reportes.Sum(r => r.Gastos),
            Total = Reportes.Sum(r => r.Ingresos) - Reportes.Sum(r => r.Gastos)
        };
    }
}
