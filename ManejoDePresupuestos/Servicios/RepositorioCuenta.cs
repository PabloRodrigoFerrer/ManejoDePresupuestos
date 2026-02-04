using Dapper;
using ManejoDePresupuestos.Models;
using Microsoft.Data.SqlClient;

namespace ManejoDePresupuestos.Servicios
{
    public interface IRepositorioCuenta
    {
        Task Create(Cuenta cuenta);
        Task<IEnumerable<Cuenta>> ObtenerCuentas(int usuarioId);
    }

    public class RepositorioCuenta : IRepositorioCuenta
    {
        private readonly string? _connectionString;
        public RepositorioCuenta(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task Create(Cuenta cuenta)
        {
            string query = @"insert into Cuentas (Nombre, TipoCuentaId, Balance,Descripcion)
                            values (@nombre, @tipoCuentaId, @balance, @descripcion)
                            select SCOPE_IDENTITY()";

            using var connection = new SqlConnection(_connectionString);
            int id = connection.QuerySingle<int>(query, cuenta);
           
            cuenta.Id = id;
        }

        //crear query para listar cuentas con su tipo cuenta y agregar el tipo cuenta como campo string. Ademas crear viewmodel para listar las cuentas con su tc
        public async Task<IEnumerable<Cuenta>> ObtenerCuentas(int usuarioId)
        {
            string query = @"select Cuentas.Nombre, Balance, tc.Nombre as TipoCuenta
                              from Cuentas
                              inner join TiposCuentas as tc 
                              on tc.Id = TipoCuentaId
                              where tc.UsuarioId = @usuarioId";

            using var connection= new SqlConnection(_connectionString);
            var result = await connection.QueryAsync<Cuenta>(query, new { usuarioId });
            return result;
        }
    
    }
}
