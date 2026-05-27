using System.Reflection.Metadata.Ecma335;
using shappo0.Enums;

namespace shappo0.Models
{
    public class Order
    {
        public int OrderNumber { get; set; }
        public DateTime TimeCreated { get; set; }
        public int WaitingTimeMinutes { get; set; }
        public OrderStatus Status { get; set; }
        public Visit Visit { get; set; }
        public Staff Staff { get; set; }
        public List<OrderLine> OrderLines { get; set; }

        public Order()
        {

        }

        public Order(int orderNumber, DateTime timeCreated, int waitingTimeMinutes, OrderStatus status, Visit visit, Staff staff)
        {
            OrderNumber = orderNumber;
            TimeCreated = timeCreated;
            WaitingTimeMinutes = waitingTimeMinutes;
            Status = status;
            Visit = visit;
            Staff = staff;
            OrderLines = new List<OrderLine>();
        }
    }

}
