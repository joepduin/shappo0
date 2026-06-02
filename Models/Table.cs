namespace shappo0.Models
{
    public class Table
    {
        public int TableNumber { get; set; }
        public bool Status { get; set; } //true is bezet false is vrij

        public Table()
        {
        }

        public Table(int tableNumber, bool status)
        {
            TableNumber = tableNumber;
            Status = status;
        }
    }
}
