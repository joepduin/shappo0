using shappo0.Enums;
using shappo0.Models;

namespace shappo0.Repositories
{
    public interface IOrderRepository
    {
        public List<Order> GetOrdersByVisitId(int visitId);

        public List<Order> GetAllOrders();

        public List<Order> GetRunningOrders();
        public List<Order> GetFinishedOrders();

        public List<Order> GetRunningOrdersByType(ProductType type);

        public int CreateOrder(Order order);

        public void UpdateOrderStatus(int orderNumber, OrderStatus status);
    }

}
