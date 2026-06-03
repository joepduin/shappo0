using System.Reflection.Metadata.Ecma335;
using shappo0.Enums;

namespace shappo0.Models
{
    public class Order
    {
        public int OrderNumber { get; set; }
        public DateTime TimeCreated { get; set; }
        public int WaitingTimeMinutes { 
            get 
            {
                TimeSpan span = DateTime.Now.Subtract(TimeCreated);
                int waitingTime = (int)span.TotalMinutes;
                return waitingTime;
            set { } }
        }
        public OrderStatus Status { get; set; }
        public Visit Visit { get; set; }
        public Staff Staff { get; set; }
        public List<OrderLine> OrderLines { get; set; }
        

        public Order()
        {

        }

        public Order(int orderNumber, DateTime timeCreated, OrderStatus status, Visit visit, Staff staff)
        {
            OrderNumber = orderNumber;
            TimeCreated = timeCreated;
            Status = status;
            Visit = visit;
            Staff = staff;
            OrderLines = new List<OrderLine>();
        }
    }

}
