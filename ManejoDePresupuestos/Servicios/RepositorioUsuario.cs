namespace ManejoDePresupuestos.Servicios
{

    public interface IRepositorioUsuario
    {
        Task<int> ObtenerUsuarioId();
    }
    public class RepositorioUsuario(IConfiguration configuration) : IRepositorioUsuario
    {
        private readonly string? _connectionString = configuration.GetConnectionString("DefaultConnection");

        public async Task<int> ObtenerUsuarioId()
        {
            return 1;
        }
    }
}
