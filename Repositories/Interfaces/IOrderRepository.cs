using shappo0.Models;

namespace shappo0.Repositories
{
    public interface IOrderRepository
    {
        public List<Order> GetOrdersByVisitId(int visitId);

        public List<Order> GetAllOrders();

        public List<Order> GetRunningOrders();

        public int CreateOrder(Order order);
    }

}
