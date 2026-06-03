using shappo0.ViewModels;

namespace shappo0.Services
{
    public interface ITableService
    {
        List<TableViewModel> GetAllTables();
        TableViewModel GetTableByNumber(int tableNumber);
    }
}
