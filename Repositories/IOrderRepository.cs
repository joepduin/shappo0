using shappo0.Models;

namespace shappo0.Repositories
{
    namespace Chapeau.Repositories
    {
        public interface IOrderRepository
        {
            Order GetOrderForTable(int tableId);
        }
    }
}
