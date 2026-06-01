using shappo0.Models;
using shappo0.Enums;

namespace shappo0.Services
{
    public interface IKitchenService 
    {
        List<Kitchen> GetAll();
        Kitchen? GetById(int OrderNumber);

        public List<OrderLine> GetOrderLines(int orderNumber);

        void UpdateOrderStatus(int orderNumber, OrderStatus status);

        void UpdateOrderLineStatus(int orderLineId, OrderStatus status);

        List<Kitchen> FinishedOrders();
    }
}
