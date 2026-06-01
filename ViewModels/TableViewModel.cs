using shappo0.Enums;

namespace shappo0.ViewModels
{
    public class TableViewModel
    {
        public int TableNumber { get; set; }
        public bool Status { get; set; }
        public OrderStatus DrinkStatus { get; set; }
        public OrderStatus FoodStatus { get; set; }

        public TableViewModel()
        {
        }

        public TableViewModel(int tableNumber, bool status, OrderStatus drinkStatus, OrderStatus foodStatus)
        {
            TableNumber = tableNumber;
            Status = status;
            DrinkStatus = drinkStatus;
            FoodStatus = foodStatus;
        }
    }
}