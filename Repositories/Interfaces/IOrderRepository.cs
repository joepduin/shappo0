using shappo0.Enums;
using shappo0.ViewModels;

namespace shappo0.Repositories
{
    public interface IOrderRepository
    {
        public List<OrderViewModel> GetOrdersByVisitId(int visitId);

        public List<OrderViewModel> GetAllOrders();

        public List<OrderViewModel> GetRunningOrders(OrderStatus status, OrderStatus? status2);

        public List<OrderViewModel> GetRunningOrdersByType(ProductType type);

        public List<OrderViewModel> GetFinishedOrdersByType(ProductType type);

        public int CreateOrder(OrderViewModel order);

        public void UpdateOrderStatus(int orderNumber, OrderStatus status);
    }

}
