using Dapper;
using ManejoDePresupuestos.Models;
using Microsoft.Data.SqlClient;

namespace ManejoDePresupuestos.Servicios
{
    public interface IRepositorioCategoria
    {
        Task Agregar(Categoria categoria);
        Task Borrar(int id);
        Task Editar(Categoria categoria);
        Task<Categoria?> ObtenerCategoriaPorId(int id, int usuarioId);
        Task<IEnumerable<Categoria>> ObtenerCategorias(int usuarioId);
    }

    public class RepositorioCategoria(IConfiguration configuration) : IRepositorioCategoria
    {
        private readonly string? _connectionString = configuration.GetConnectionString("DefaultConnection");

        public async Task Agregar(Categoria categoria)
        {
            string query = @"insert into Categorias (Nombre, TipoOperacionId, UsuarioId)
                                values (@nombre, @tipoOperacionId, @usuarioId)";

            var connection = new SqlConnection(_connectionString);
            await connection.ExecuteAsync(query, categoria);
        }

        public async Task<IEnumerable<Categoria>> ObtenerCategorias(int usuarioId)
        {
            string query = @"select *
                            from Categorias
                            where UsuarioId = @usuarioId";

            var connection = new SqlConnection(_connectionString);
            var categorias = await connection.QueryAsync<Categoria>(query, new { usuarioId });

            return categorias;
        }

        public async Task<Categoria?> ObtenerCategoriaPorId(int id, int usuarioId)
        {
            string query = @"select * 
                            from Categorias
                            where Id = @id and UsuarioId = @usuarioId";

            var connection = new SqlConnection(_connectionString);
            return await connection.QueryFirstOrDefaultAsync<Categoria>(query, new {id, usuarioId});
        }

        public async Task Editar(Categoria categoria)
        {
            string query = @"update Categorias
                                set Nombre = @nombre, TipoOperacionId = @tipoOperacionId
                                where Id = @id";

            var connection = new SqlConnection(_connectionString);
            await connection.ExecuteAsync(query, categoria);
        }

        public async Task Borrar(int id)
        {
            string query = @"delete from Categorias
                             where Id = @id";

            var connection = new SqlConnection(_connectionString);
            await connection.ExecuteAsync(query, new {id});
        }


    }
}
