using shappo0.Enums;
using shappo0.Models;

namespace shappo0.Services
{
    public interface IOrderService
    {
        public List<Order> GetOrdersByVisitId(int visitId);
        public List<Order> GetAllOrders();
        public List<Order> GetRunningOrders(OrderStatus status);

        public List<Order> GetRunningOrdersByType(ProductType type);
        public List<OrderLine> GetOrderLinesByOrderNumber(int orderNumber);
        public List<OrderLine> GetOrderLinesByType(int orderNumber, ProductType type);
        public void CreateOrder(int tableNumber, int staffId, List<OrderLine> orderLines);
        void AddProductToCart(List<OrderLine> cart, Product selectedProduct);
        bool TryIncreaseQuantity(List<OrderLine> cart, int productId);
        void DecreaseQuantity(List<OrderLine> cart, int productId);
        void RemoveLine(List<OrderLine> cart, int productId);
        void AddNote(List<OrderLine> cart, int productId, string note);

        void UpdateOrderLineStatus(int orderLineId, OrderStatus status);
        void UpdateOrderStatus(int orderNumber, OrderStatus status);
        void UpdateCourseStatus(int orderNumber, ProductType productType, OrderStatus status);

    }
}
