using shappo0.Enums;

namespace shappo0.Models
{
    public class Kitchen
    {
        private string status;

        public int OrderNumber { get; set; }
        public int Tablenumber { get; set; }

        public OrderStatus Status { get; set; } // running, prepared, completed (3)
        public DateTime TimeCreated { get; set; }

        public int WaitingTime { get; set; }


        public Kitchen(int orderNumber, int tableNumber, OrderStatus status, DateTime timeCreated, int waitingTime)
        {
            OrderNumber = orderNumber;
            Tablenumber = tableNumber;
            Status = Enum.Parse<OrderStatus>(status.ToString(), true);
            TimeCreated = timeCreated;
            WaitingTime = waitingTime;
        }

        public Kitchen() { }

        public Kitchen(int orderNumber, int tableNumber, string status, DateTime timeCreated, int waitingTime)
        {
            OrderNumber = orderNumber;
            Tablenumber = tableNumber;
            this.status = status;
            TimeCreated = timeCreated;
            WaitingTime = waitingTime;
        }
    }
}
