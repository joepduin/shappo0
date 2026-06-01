namespace shappo0.Models
{
    public class Finance
    {
        public string MenuType { get; set; }
        public int TotalItemsOrdered { get; set; }
        public decimal TotalRevenue { get; set; }
        public Finance(string menuType, int totalItemsOrdered, decimal totalRevenue)
        {
            MenuType = menuType;
            TotalItemsOrdered = totalItemsOrdered;
            TotalRevenue = totalRevenue;
        }
    }
}
