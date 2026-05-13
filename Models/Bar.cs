namespace shappo0.Models
{
    public class Bar
    {
        public int OrderNumber { get; set; }
        List<OrderLine> Orderlines { get; set; }
        public string Status { get; set; }
        public DateTime TimeCreated { get; set; }

        public DateTime WaitingTime { get; set; }

        public Bar(int orderNumber, List<OrderLine> orderlines, string status, DateTime timeCreated, DateTime waitingTime)
        {
            OrderNumber = orderNumber;
            Orderlines = orderlines;
            Status = status;
            TimeCreated = timeCreated;
            WaitingTime = waitingTime;

        }

        public Bar() { }

    }
}
