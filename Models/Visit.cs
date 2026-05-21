namespace shappo0.Models
{
    public class Visit
    {
        public int VisitId { get; set; }
        public int TableNumber { get; set; }
        public int StaffId { get; set; }
        public bool IsActive { get; set; }
        public List<Order> Orders { get; set; }

        public Visit(int visitId, int tableNumber, int staffId, bool isActive)
        {
            VisitId = visitId; 
            TableNumber = tableNumber;
            StaffId = staffId;
            IsActive = isActive;
            Orders = new List<Order>();
        }

        public Visit()
        {

        }
    }




}
