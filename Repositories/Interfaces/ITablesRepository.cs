using shappo0.Models;

namespace shappo0.Repositories
{
    public interface ITablesRepository
    {
        List<Table> GetAllTables();
        Table? GetTableByNumber(int tableNumber);
        void ChangeTableStatus(Table table);
    }
}
