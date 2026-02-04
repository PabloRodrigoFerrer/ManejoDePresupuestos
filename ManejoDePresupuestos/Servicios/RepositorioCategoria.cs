using Dapper;
using ManejoDePresupuestos.Models;
using Microsoft.Data.SqlClient;

namespace ManejoDePresupuestos.Servicios
{
    public interface IRepositorioCategoria
    {
        Task Agregar(Categoria categoria);
    }

    public class RepositorioCategoria : IRepositorioCategoria
    {
        private readonly string? _connectionString;

        public RepositorioCategoria(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task Agregar(Categoria categoria)
        {
            string query = @"insert into Categorias (Nombre, TipoOperacionId, UsuarioId)
                                values (@nombre, @tipoOperacionId, @usuarioId)";

            var connection = new SqlConnection(_connectionString);
            await connection.ExecuteAsync(query, categoria);
        }

    }
}
