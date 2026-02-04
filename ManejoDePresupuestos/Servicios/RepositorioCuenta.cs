using Dapper;
using ManejoDePresupuestos.Models;
using Microsoft.Data.SqlClient;

namespace ManejoDePresupuestos.Servicios
{
    public interface IRepositorioCuenta
    {
        Task Borrar(int id);
        Task Create(Cuenta cuenta);
        Task Editar(Cuenta cuentaEditar);
        Task<Cuenta?> ObtenerCuentaPorId(int id, int usuarioId);
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
            string query = @"select Cuentas.Id, Cuentas.Nombre, Balance, tc.Nombre as TipoCuenta
                              from Cuentas
                              inner join TiposCuentas as tc 
                              on tc.Id = TipoCuentaId
                              where tc.UsuarioId = @usuarioId";

            using var connection= new SqlConnection(_connectionString);
            var result = await connection.QueryAsync<Cuenta>(query, new { usuarioId });
            return result;
        }

        public async Task<Cuenta?> ObtenerCuentaPorId(int id, int usuarioId)
        {
            string query = @"select Cuentas.Id, Cuentas.Nombre, Balance, Descripcion, TipoCuentaId
                              from Cuentas
                              inner join TiposCuentas as tc 
                              on tc.Id = TipoCuentaId
                              where tc.UsuarioId = @usuarioId and Cuentas.Id = @id";

            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryFirstOrDefaultAsync<Cuenta>(query, new { id, usuarioId });
        }
    
        public async Task Editar(Cuenta cuentaEditar)
        {
            string query = @"update Cuentas
                              set Nombre = @nombre,
                              TipoCuentaId = @tipoCuentaId,
                              Balance = @balance,
                              Descripcion = @descripcion
                              where Id = @Id ";
        
            using var connection = new SqlConnection(_connectionString);
            await connection.ExecuteAsync(query, cuentaEditar);

        }

        public async Task Borrar(int id)
        {
            string query = "delete from Cuentas where Id = @id";
            using var connection = new SqlConnection(_connectionString);
            await connection.ExecuteAsync(query, new { id });
        }
    }
}
