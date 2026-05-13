namespace shappo0.Models
{
    public class OrderLine
    {
        public int OrderLineId { get; set; }
        public int Quantity { get; set; }
        public string Note { get; set; }


        public OrderLine(int orderLineId, int quantity, string note)
        {
            OrderLineId = orderLineId;
            Quantity = quantity;
            Note = note;
        }

        public OrderLine() { }
    }
}
