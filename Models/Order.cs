namespace shappo0.Models
{
    public class Order
    {
        public int Id { get; set; }
        public int TableId { get; set; }
        public List<OrderLine> OrderItems { get; set; }

        public Order()
        {
        }

        public Order(int id, int tableId, List<OrderLine> orderItems)
        {
            Id = id;
            TableId = tableId;
            OrderItems = orderItems;
        }
    }


}
