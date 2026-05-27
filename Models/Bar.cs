using shappo0.Enums;

namespace shappo0.Models
{
    public class Bar
    {
        public int OrderNumber { get; set; }
        public int Tablenumber { get; set; } 
        
        public OrderStatus Status { get; set; } // running, completed (2)
        public DateTime TimeCreated { get; set; }

        public int WaitingTime { get; set; }

        public Bar(int orderNumber,int tableNumber, OrderStatus status, DateTime timeCreated, int waitingTime)
        {
            OrderNumber = orderNumber;
            Status = Enum.Parse<OrderStatus>(status.ToString(), true); 
            TimeCreated = timeCreated;
            WaitingTime = waitingTime;

        }

        public Bar() { }

        public Bar(int orderNumber, int tableNumber, string status, DateTime timeCreated, int waitingTime)
        {
            OrderNumber = orderNumber;
            Tablenumber = tableNumber;
            Status = Enum.Parse<OrderStatus>(status, true);
            TimeCreated = timeCreated;
            WaitingTime = waitingTime;
        }

    }
}
