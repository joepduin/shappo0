using shappo0.Models;

namespace shappo0.Repositories
{
    public class DbFinanceRepository : IFinanceRepository
    {
        private readonly string _connectionString;
        public DbFinanceRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("shappo0");
        }

        public List<Bill> GetFinances()
        {
            throw new NotImplementedException();
        }
        public List<Bill> GetFinancesByDate(DateTime date)
        {
            throw new NotImplementedException();

        }
    }
}
