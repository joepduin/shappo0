namespace shappo0.Models
{
    public class Order
    {
        public int Id { get; set; }
        public int TableId { get; set; }
        public List<OrderItem> OrderItems { get; set; }

        public Order()
        {
        }

        public Order(int id, int tableId, List<OrderItem> orderItems)
        {
            Id = id;
            TableId = tableId;
            OrderItems = orderItems;
        }
    }


}
