using shappo0.Models;

namespace shappo0.Services
{
    public interface IBarService
    {
        List<Bar> GetAll();
        Bar? GetById(int OrderNumber);

        public List<OrderLine> GetOrderLines(int orderNumber);

        void UpdateOrderStatus(int orderNumber, OrderStatus status);

        void UpdateOrderLineStatus(int orderLineId, OrderStatus status);

        List<Bar> FinishedOrders();
    }
}
