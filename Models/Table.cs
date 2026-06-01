namespace shappo0.Models
{
    public class Table
    {
        public int TableNumber { get; set; }
        public bool Status { get; set; } //true is bezet false is vrij
        public List<Order> Orders { get; set; }

        public Table()
        {
        }

        public Table(int tableNumber, bool status, List<Order> orders)
        {
            TableNumber = tableNumber;
            Status = status;
            Orders = orders;
        }
    }
}
