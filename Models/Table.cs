namespace shappo0.Models
{
    public class Table
    {
        public int TableNumber { get; set; }
        public TableStatusType Status { get; set; }

        public OrderStatus? DrinkStatus { get; set; }

        public OrderStatus? FoodStatus { get; set; }

        public Table()
        {
        }

        public Table(int tableNumber, TableStatusType status, OrderStatus? drinkstatus, OrderStatus? foodStatus)
        {
            TableNumber = tableNumber;
            Status = status;
            DrinkStatus = drinkstatus;
            FoodStatus = foodStatus;
        }
    }
}