namespace ManejoDePresupuestos.Servicios
{

    public interface IRepositorioUsuario
    {
        Task<int> ObtenerUsuarioId();
    }
    public class RepositorioUsuario : IRepositorioUsuario
    {
        private readonly string? _connectionString;
        public RepositorioUsuario(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<int> ObtenerUsuarioId()
        {
            return 1;
        }
    }
}
