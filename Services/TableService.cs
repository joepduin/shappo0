using shappo0.Models;
using shappo0.Repositories;

namespace shappo0.Services
{
    public class TableService : ITableService
    {
        private readonly ITablesRepository _tablesRepository;

        public TableService(ITablesRepository tablesRepository)
        {
            _tablesRepository = tablesRepository;
        }

        public List<Table> GetAllTables()
        {
            return _tablesRepository.GetAllTables();
        }
    }
}
