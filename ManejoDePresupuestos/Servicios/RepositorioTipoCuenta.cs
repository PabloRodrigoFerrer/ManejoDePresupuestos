using Dapper;
using ManejoDePresupuestos.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace ManejoDePresupuestos.Servicios
{
    public interface IRepositorioTipoCuenta
    {
        Task Actualizar(TipoCuenta tipoCuenta);
        Task Create(TipoCuenta tipoCuenta);
        Task<bool> Existe(string nombre, int usuarioId);
        Task<IEnumerable<TipoCuenta>> ObtenerCuentasPorUsuario(int usuarioId);
        Task<TipoCuenta?> ObtenerPorId(int id, int usuarioId);
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

        public async Task<bool> Existe(string nombre, int usuarioId)
        {
            using var connection = new SqlConnection(_connectionString);
            var existe = await connection.QueryFirstOrDefaultAsync<int>
                (@" select 1 from TiposCuentas
                    where Nombre = @Nombre and UsuarioId = @UsuarioId;",
                    new { nombre, usuarioId });

            return existe == 1;
        }

        public async Task<IEnumerable<TipoCuenta>> ObtenerCuentasPorUsuario(int usuarioId)
        {
             using var connection = new SqlConnection(_connectionString);
             var cuentas = await connection.QueryAsync<TipoCuenta>
               (@"select Id, Nombre, Orden
                from TiposCuentas
                where UsuarioId = @usuarioId;", new { usuarioId });

            return cuentas;
        }

        public async Task Actualizar(TipoCuenta tipoCuenta)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.ExecuteAsync
                (@"update TiposCuentas
                set Nombre = @nombre
                where Id = @id", tipoCuenta);
        }
        
        public async Task<TipoCuenta?> ObtenerPorId(int id, int usuarioId)
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryFirstOrDefaultAsync<TipoCuenta>
                (@"select Id, Nombre, Orden
                from TiposCuentas
                where Id = @id and UsuarioId = @usuarioId",
                new {id, usuarioId});                
        }
    }
}
