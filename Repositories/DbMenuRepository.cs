namespace shappo0.Repositories
{
    public class DbMenuRepository
    {
        private readonly string? _connectionString;

        public DbMenuRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("Shappo0");
        }
        
        public void GetAllDocuments
    }
}
