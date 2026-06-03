using shappo0.Enums;
using shappo0.Models;

namespace shappo0.Repositories
{
    public interface IOrderLineRepository
    {

        public List<OrderLine> GetOrderLinesByOrderNumber(int orderNumber);

        public List<OrderLine> GetActiveOrderLinesByTableNumber(int tableNumber);

        public void CreateOrderLine(OrderLine orderLine);

        public void CreateOrderLines(List<OrderLine> orderLines);
        public void UpdateOrderLineStatus(int orderLineId, OrderStatus status);
    }
}
