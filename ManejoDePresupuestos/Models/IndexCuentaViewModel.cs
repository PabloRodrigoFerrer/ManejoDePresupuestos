namespace ManejoDePresupuestos.Models
{
    public class IndexCuentaViewModel
    {
        public string TipoCuenta { get; set; } = string.Empty;

        public IEnumerable<Cuenta> Cuentas { get; set; } = [];

        public decimal Balance => Cuentas.Sum(x => x.Balance);

        public string AsignarClaseBalance(decimal balance) => balance > 0 ? "activo" : "pasivo";
    }
}
