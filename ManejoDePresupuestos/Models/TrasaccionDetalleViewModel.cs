namespace ManejoDePresupuestos.Models
{
    public class TransaccionesPorCuentaViewModel
    {
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public int CuentaId { get; set; }
        public int MesAnterior => FechaInicio.AddMonths(-1).Month;
        public int AñoAnterior => FechaInicio.AddMonths(-1).Year;
        public int MesPosterior => FechaInicio.AddMonths(1).Month;
        public int AñoPosterior => FechaInicio.AddMonths(1).Year;
        public IEnumerable<TransaccionesPorFecha> TransaccionesAgrupadas { get; set; } = [];
        public decimal BalanceIngresos => TransaccionesAgrupadas.Sum(t => t.BalanceIngreso);
        public decimal BalanceGastos => TransaccionesAgrupadas.Sum(t => t.BalanceGasto);
        public decimal Total => BalanceIngresos - BalanceGastos;
        public string? UrlRetorno = string.Empty;
        public string AsignarClaseBalance(TipoOperacion operacion) => operacion is TipoOperacion.Ingreso ? "activo" : "pasivo";
    }

    public class TransaccionesPorFecha
    {
        public DateTime FechaTransaccion { get; set; }

        public IEnumerable<TransaccionDetalleDTO> Transacciones { get; set; } = [];

        public decimal BalanceIngreso => 
            Transacciones
            .Where(t => t.TipoOperacion == TipoOperacion.Ingreso)
            .Sum(t => t.Monto);

        public decimal BalanceGasto =>
         Transacciones
         .Where(t => t.TipoOperacion == TipoOperacion.Gasto)
         .Sum(t => t.Monto);
    }

    public class TransaccionDetalleDTO
    {
        public int Id { get; set; }
        public DateTime FechaTransaccion { get; set; }
        public decimal Monto { get; set; }
        public string Nota { get; set; } = string.Empty;
        public string Categoria { get; set; } = string.Empty;
        public TipoOperacion TipoOperacion { get; set; }
    }
}
