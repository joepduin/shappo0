using shappo0.Models;
using shappo0.ViewModels;

namespace shappo0.Services
{
    public interface ITableService
    {
        List<TableStatusViewModel> GetAllTables();
        List<TableViewModel> GetOrdersForTable(int tableNummer);
    }
}
