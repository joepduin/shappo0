using shappo0.Enums;

namespace shappo0.Models
{
    public class Bar
    {
        public int OrderNumber { get; set; }
        public int TableNumber { get; set; }
        public OrderStatus Status { get; set; } 
        public DateTime TimeCreated { get; set; }
        public int WaitingTime { get; set; }
        public List<OrderLine> OrderLines { get; set; }

        public Bar(int orderNumber,int tableNumber, OrderStatus status, DateTime timeCreated, int waitingTime)
        {
            OrderNumber = orderNumber;
            TableNumber = tableNumber;
            Status = status; 
            TimeCreated = timeCreated;
            WaitingTime = waitingTime;

        }

        public Bar() { }

        public Bar(int orderNumber, int tableNumber, string status, DateTime timeCreated, int waitingTime)
        {
            OrderNumber = orderNumber;
            TableNumber = tableNumber;
            Status = Enum.Parse<OrderStatus>(status, true);
            TimeCreated = timeCreated;
            WaitingTime = waitingTime;
        }

    }
}
