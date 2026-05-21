namespace shappo0.Models
{
    public class Table
    {
        public int TableNumber { get; set; }
        public TableStatusType Status { get; set; }

        public Table()
        {
        }

        public Table(int tableNumber, TableStatusType status)
        {
            TableNumber = tableNumber;
            Status = status;
        }
    }
}