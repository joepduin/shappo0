using shappo0.Models;

namespace shappo0.Services
{
    public interface IOrderService
    {
        public List<Order> GetOrdersByVisitId(int visitId);
        public List<Order> GetAllOrders();
        public List<Order> GetRunningOrders();
        public List<OrderLine> GetOrderLinesByOrderNumber(int orderNumber);
        public void CreateOrder(int tableNumber, int staffId, List<OrderLine> orderLines);

    }
}
