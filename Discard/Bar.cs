using shappo0.Enums;
using shappo0.Models;

namespace shappo0.Discard
{
    public class Bar
    {
        public int OrderNumber { get; set; }
        public int TableNumber { get; set; }
        public OrderStatus Status { get; set; } 
        public DateTime TimeCreated { get; set; }
        public int WaitingTime
        {
            get
            {
                TimeSpan span = DateTime.Now.Subtract(TimeCreated);
                int waitingTime = (int)span.TotalMinutes;
                return waitingTime;
            }
            set { }
        }
        public List<OrderLine> OrderLines { get; set; }

        public Bar(int orderNumber,int tableNumber, OrderStatus status, DateTime timeCreated)
        {
            OrderNumber = orderNumber;
            TableNumber = tableNumber;
            Status = status; 
            TimeCreated = timeCreated;

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
