namespace shappo0.Models
{
    public class OrderLine
    {
        public int OrderLineId { get; set; }
        public int ProductId { get; set; }
        public int OrderNumber { get; set; }
        public string Note { get; set; }
        public int Quantity { get; set; }
        public OrderStatus Status { get; set; }

        public OrderLine(int orderLineId, int productId, int orderNumber, string note, int quantity, OrderStatus status)
        {
            OrderLineId = orderLineId;
            ProductId = productId;
            OrderNumber = orderNumber;
            Note = note;
            Quantity = quantity;
            Status = status;
        }

        public OrderLine()
        {

        }

        //enum status per item --> ---, being prepped, ready to be served (3) --> add column to database






    }
}
