using Dapper;
using ManejoDePresupuestos.Models;
using Microsoft.Data.SqlClient;

namespace ManejoDePresupuestos.Servicios
{
    public interface IRepositorioTipoCuenta
    {
        Task Create(TipoCuenta tipoCuenta);
    }

    public class RepositorioTipoCuenta : IRepositorioTipoCuenta
    {
        private readonly string? _connectionString;

        public RepositorioTipoCuenta(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task Create(TipoCuenta tipoCuenta)
        {
            using var connection = new SqlConnection(_connectionString);

            int id = connection.QuerySingle<int>
                ("insert into TiposCuentas (Nombre, UsuarioId, Orden) " +
                "values (@Nombre, @UsuarioId, 0) " +
                "select SCOPE_IDENTITY();"
                , tipoCuenta);

            tipoCuenta.Id = id;



        }

    }
}
