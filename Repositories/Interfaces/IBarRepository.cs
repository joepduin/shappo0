using shappo0.Models;
using shappo0.Enums;

namespace shappo0.Repositories
{
    public interface IBarRepository
    {
        List<Bar> GetAll();
        Bar? GetById(int OrderNumber);
        public List<OrderLine> GetOrderLines(int orderNumber);

        void UpdateOrderStatus(int orderNumber, OrderStatus status);

        void UpdateOrderLineStatus(int orderLineId, OrderStatus status);
        List<Bar> FinishedOrders();


    }
}
