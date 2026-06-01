using shappo0.Enums;

namespace shappo0.Models
{
    public class Kitchen
    {

        public int OrderNumber { get; set; }
        public int TableNumber { get; set; }
        public OrderStatus Status { get; set; } 
        public DateTime TimeCreated { get; set; }
        public int WaitingTime { get; set; }
        public List<OrderLine> OrderLines { get; set; }


        public Kitchen(int orderNumber, int tableNumber, OrderStatus status, DateTime timeCreated)
        {

            OrderNumber = orderNumber;
            TableNumber = tableNumber;
            Status = status;
            TimeCreated = timeCreated;
            OrderLines = new List<OrderLine>();
        }

        public Kitchen() { }

        public Kitchen(int orderNumber, int tableNumber, string status, DateTime timeCreated, int waitingTime)
        {
            OrderNumber = orderNumber;
            TableNumber = tableNumber;
            Status = Enum.Parse<OrderStatus>(status.ToString(), true);
            TimeCreated = timeCreated;
            WaitingTime = waitingTime;
        }
    }
}
