namespace shappo0.Models
{
    public class OrderLine
    {
        public int OrderLineId { get; set; }
        public int Quantity { get; set; }
        public string Note { get; set; }
        public Product Product { get; set; }

        public OrderStatus Status { get; set; }


        public OrderLine(int orderLineId, int quantity, string note, OrderStatus status)
        {
            OrderLineId = orderLineId;
            Quantity = quantity;
            Note = note;
            Status = status;
        }

        public OrderLine(int orderLineId, int quantity, string note, Product product)
        {
            OrderLineId = orderLineId;
            Quantity = quantity;
            Note = note;
            Product = product;
        }

        public OrderLine() { }
    }
}
