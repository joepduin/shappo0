namespace shappo0.Models
{
    public class Bar
    {
        public int OrderNumber { get; set; }
        public int Tablenumber { get; set; } //add tableNumber to order database
        List<OrderLine> Orderlines { get; set; }
        public OrderStatus Status { get; set; } // running, completed (2)
        public DateTime TimeCreated { get; set; }

        public DateTime WaitingTime { get; set; }

        public Bar(int orderNumber,int tableNumber, List<OrderLine> orderlines, OrderStatus status, DateTime timeCreated, DateTime waitingTime)
        {
            OrderNumber = orderNumber;
            Tablenumber = tableNumber;
            Orderlines = orderlines;
            Status = status;
            TimeCreated = timeCreated;
            WaitingTime = waitingTime;

        }

        public Bar() { }

    }
}
