using shappo0.Models;

namespace shappo0.Repositories
{
    public interface IOrderRepository
    {
        Order GetOrderForTable(int tableId);
    }
}
