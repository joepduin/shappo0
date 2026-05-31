using shappo0.Enums;

namespace shappo0.Models
{
    public class OrderLine
    {
        public int OrderLineId { get; set; }
        public Product Product { get; set; }
        public Order Order { get; set; }
        public string? Note { get; set; }
        public int Quantity { get; set; }
        public OrderStatus Status { get; set; }

        public OrderLine(int orderLineId, Product product, Order order, string? note, int quantity, OrderStatus status)
        {
            OrderLineId = orderLineId;
            Product = product;
            Order = order;
            Note = note;
            Quantity = quantity;
            Status = status;
        }

        public OrderLine()
        {

        }

        






    }
}
