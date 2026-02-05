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
    }
}
