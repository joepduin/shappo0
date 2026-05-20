using System.Reflection.Metadata.Ecma335;

namespace shappo0.Models
{
    public class Order
    {
        public int OrderNumber { get; set; }
        public DateTime TimeCreated { get; set; }
        public DateTime waitingTime { get; set; }
        public OrderStatus Status { get; set; }
        public int VisitId { get; set; }
        public int StaffId { get; set; }

        public Order()
        {
        }

        public Order(int orderNumber, DateTime timeCreated, DateTime waitingTime, OrderStatus status, int visitId, int staffId)
        {
            OrderNumber = orderNumber;
            TimeCreated = timeCreated;
            this.waitingTime = waitingTime;
            Status = status;
            VisitId = visitId;
            StaffId = staffId;
        }
    }


}
