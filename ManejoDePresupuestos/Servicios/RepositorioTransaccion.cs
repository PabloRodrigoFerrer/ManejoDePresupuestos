using Dapper;
using ManejoDePresupuestos.Models;
using Microsoft.Data.SqlClient;
using static ManejoDePresupuestos.Servicios.RepositorioTransaccion;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ManejoDePresupuestos.Servicios
{
    public interface IRepositorioTransaccion
    {
        Task Agregar(Transaccion transaccion);
        Task Borrar(int id);
        Task Editar(Transaccion transaccion, int cuentaAnteriorId, decimal montoAnterior);
        Task<Transaccion?> ObtenerPorId(int id, int usuarioId);
        Task<TransaccionEditarViewModel?> ObtenerPorIdConTipoOperacion(int id, int usuarioId);
    }
    public class RepositorioTransaccion(IConfiguration configuration) : IRepositorioTransaccion
    {
        private readonly string? _connectionString = configuration.GetConnectionString("DefaultConnection");

        public async Task Agregar(Transaccion transaccion)
        {
            //Insertamos transaccion y actualizamos el balance de la tabla cuentas.
            string storeProcedure = "Transacciones_Insertar";

            using var connection = new SqlConnection(_connectionString);
            int id = await connection.QuerySingleAsync<int>(
                        storeProcedure,
                         new
                         {
                             usuarioId = transaccion.UsuarioId,
                             fechaTransaccion = transaccion.FechaTransaccion,
                             monto = transaccion.Monto,
                             nota = transaccion.Nota,
                             cuentaId = transaccion.CuentaId,
                             categoriaId = transaccion.CategoriaId
                         },
                        commandType: System.Data.CommandType.StoredProcedure);

            transaccion.Id = id;
        }

        public async Task Editar(Transaccion transaccion, int cuentaIdAnterior, decimal montoAnterior)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.ExecuteAsync(
                "Transacciones_Editar",
                new
                {
                    transaccion.Id,
                    transaccion.FechaTransaccion,
                    transaccion.Monto,
                    transaccion.Nota,
                    transaccion.CuentaId,
                    transaccion.CategoriaId,
                    montoAnterior,
                    cuentaIdAnterior
                },
                commandType: System.Data.CommandType.StoredProcedure);
        }

        public async Task<Transaccion?> ObtenerPorId(int id, int usuarioId)
        {
            string query = @"select * from Transacciones
                            where Id = @id and UsuarioId = @usuarioId";

            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryFirstOrDefaultAsync<Transaccion>(
                query, new { id, usuarioId });
        }

        public async Task<TransaccionEditarViewModel?> ObtenerPorIdConTipoOperacion(int id, int usuarioId)
        {
            string query = @"select tra.*, cat.TipoOperacionId 
                              from Transacciones as Tra
                              inner join Categorias as cat
                              on cat.Id = Tra.CategoriaId
                              where Tra.Id = @id and Tra.UsuarioId= @usuarioId";

            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryFirstOrDefaultAsync<TransaccionEditarViewModel>
                (query, new { id, usuarioId });
        } 

        public async Task Borrar(int id) // este procedimiento actualiza el balance de la cuenta según la transacción eliminada.
        {
            string storeProcedure = "Transaccion_Borrar";

            using var connection = new SqlConnection(_connectionString);

            await connection.ExecuteAsync(
                storeProcedure,
                new { id },
                commandType: System.Data.CommandType.StoredProcedure);         
        }
    }
}
