using shappo0.Models;

namespace shappo0.Repositories
{
    public interface IOrderLineRepository
    {

        public List<OrderLine> GetOrderLinesByOrderNumber(int orderNumber);

        public void CreateOrderLine(OrderLine orderLine);

        public void CreateOrderLines(List<OrderLine> orderLines);
    }
}
