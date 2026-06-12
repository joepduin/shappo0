using shappo0.Enums;
using shappo0.Models;
using shappo0.ViewModels;

namespace shappo0.Services
{
    public interface IOrderService
    {
        public List<OrderViewModel> GetOrdersByVisitId(int visitId);
        public List<OrderViewModel> GetAllOrders();
        public List<OrderViewModel> GetRunningOrders(OrderStatus status, OrderStatus? status2);

        public List<OrderViewModel> GetRunningOrdersByType(ProductType type);
        public List<OrderViewModel> GetFinishedOrdersByType(ProductType type);
        public List<OrderLine> GetOrderLinesByOrderNumber(int orderNumber);
        public List<OrderLine> GetOrderLinesByType(int orderNumber, ProductType type);
        public List<OrderLine> GetActiveOrderLinesByTableNumber(int orderNumber);
        public void CreateOrder(int tableNumber, int staffId, List<OrderLine> orderLines);
        void AddProductToCart(List<OrderLine> cart, Product selectedProduct);
        bool TryIncreaseQuantity(List<OrderLine> cart, int productId);
        void DecreaseQuantity(List<OrderLine> cart, int productId);
        void RemoveLine(List<OrderLine> cart, int productId);
        void AddNote(List<OrderLine> cart, int productId, string note);

        void UpdateOrderLineStatus(OrderLine orderLine, OrderStatus status);
        void UpdateOrderStatus(int orderNumber, OrderStatus status);
        void UpdateCourseStatus(int orderNumber, ProductType productType, OrderStatus status);
        List<OrderLine> GetGroupedOrderLinesByVisitId(int visitId);
    }
}
