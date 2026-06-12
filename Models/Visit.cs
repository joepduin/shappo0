namespace shappo0.Models
{
    public class Visit
    {
        public int VisitId { get; set; }
        public Table Table { get; set; }
        public Staff Staff { get; set; }
        public bool IsActive { get; set; }
        public List<Order> Orders { get; set; }

        public Visit(int visitId, Table table, Staff staff, bool isActive)
        {
            VisitId = visitId; 
            Table = table;
            Staff = staff;
            IsActive = isActive;
            Orders = new List<Order>();
        }

        public Visit()
        {
            Staff = new();
            Table = new();
            Orders = [];
        }
    }




}
