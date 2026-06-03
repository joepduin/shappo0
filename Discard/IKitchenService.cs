using shappo0.Models;
using shappo0.Enums;

namespace shappo0.Discard
{
    public interface IKitchenService 
    {
        List<Kitchen> GetAll();
        Kitchen? GetById(int OrderNumber);

        public List<OrderLine> GetOrderLines(int orderNumber);

        void UpdateOrderStatus(int orderNumber, OrderStatus status);

        void UpdateOrderLineStatus(int orderLineId, OrderStatus status);

        List<Kitchen> FinishedOrders();

        void UpdateCourseStatus(int orderNumber, ProductType productType, OrderStatus status);
    }
}
