namespace shappo0.Models
{
    public class OrderLine
    {
        public int OrderLineId { get; set; }
        public int Quantity { get; set; }
        public string Note { get; set; }
        public Product Product { get; set; }

        //enum status per item --> ---, being prepped, ready to be served (3) --> add column to database

        public OrderLine(int orderLineId, int quantity, string note)
        {
            OrderLineId = orderLineId;
            Quantity = quantity;
            Note = note;
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
