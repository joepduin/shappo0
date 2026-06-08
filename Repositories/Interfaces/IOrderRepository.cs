using shappo0.Enums;
using shappo0.Models;

namespace shappo0.Repositories
{
    public interface IOrderRepository
    {
        public List<Order> GetOrdersByVisitId(int visitId);

        public List<Order> GetAllOrders();

        public List<Order> GetRunningOrders(OrderStatus status, OrderStatus? status2);

        public List<Order> GetRunningOrdersByType(ProductType type);

        public int CreateOrder(Order order);

        public void UpdateOrderStatus(int orderNumber, OrderStatus status);
    }

}
