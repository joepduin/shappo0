using shappo0.Models;

namespace shappo0.Repositories
{
    public interface IOrderLineRepository
    {

        public List<OrderLine> GetOrderLinesByOrderNumber(int orderNumber);
    }
}
