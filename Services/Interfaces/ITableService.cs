using shappo0.Enums;
using shappo0.ViewModels;

namespace shappo0.Services
{
    public interface ITableService
    {
        List<TableViewModel> GetAllTables();
        TableViewModel GetTableByNumber(int tableNumber);
        void UpdateOrderLineStatus(int orderLineId, OrderStatus status, int orderNumber);
        bool UpdateTableStatus(int tableNumber, bool status);
    }
}
