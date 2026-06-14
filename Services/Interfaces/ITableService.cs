using shappo0.Enums;
using shappo0.Models;
using shappo0.ViewModels;

namespace shappo0.Services
{
    public interface ITableService
    {
        Table? GetTableByNumber(int tableNumber);
        List<Table> GetAllTables();
        bool UpdateTableStatus(Table table);
        public bool CanTablePay(List<Order> orders);
    }
}
