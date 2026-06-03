using shappo0.Models;
using shappo0.ViewModels;

namespace shappo0.Services
{
    public interface ITableService
    {
        List<TableViewModel> GetAllTables();
        TableViewModel GetTableByNumber(int tableNumber);
        void UpdateOrderLineStatus(Order order);
        bool UpdateTableStatus(Table table);
    }
}
