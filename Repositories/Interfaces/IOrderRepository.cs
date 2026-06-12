using shappo0.Enums;
using shappo0.ViewModels;

namespace shappo0.Repositories
{
    public interface IOrderRepository
    {
        public List<OrderViewmodel> GetOrdersByVisitId(int visitId);

        public List<OrderViewmodel> GetAllOrders();

        public List<OrderViewmodel> GetRunningOrders(OrderStatus status, OrderStatus? status2);

        public List<OrderViewmodel> GetRunningOrdersByType(ProductType type);

        public List<OrderViewmodel> GetFinishedOrdersByType(ProductType type);

        public int CreateOrder(OrderViewmodel order);

        public void UpdateOrderStatus(int orderNumber, OrderStatus status);
    }

}
