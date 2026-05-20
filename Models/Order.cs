using System.Reflection.Metadata.Ecma335;

namespace shappo0.Models
{
    public class Order
    {
        public int OrderNumber { get; set; }
        public DateTime TimeCreated { get; set; }
        public int WaitingTimeMinutes { get; set; }
        public OrderStatus Status { get; set; }
        public int VisitId { get; set; }
        public int StaffId { get; set; }

        public Order()
        {

        }

        public Order(int orderNumber, DateTime timeCreated, int waitingTimeMinutes, OrderStatus status, int visitId, int staffId)
        {
            OrderNumber = orderNumber;
            TimeCreated = timeCreated;
            WaitingTimeMinutes = waitingTimeMinutes;
            Status = status;
            VisitId = visitId;
            StaffId = staffId;
        }
    }

}
