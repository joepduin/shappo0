using shappo0.Models;

namespace shappo0.ViewModels
{
    public class TableStatusViewModel
    {
        public int TableNumber { get; set; }
        public TableStatusType Status { get; set; }
        public TableOrderStatus DrinkStatus { get; set; }
        public TableOrderStatus FoodStatus { get; set; }

        public TableStatusViewModel()
        {

        }

        public TableStatusViewModel(int tableNumber, TableStatusType status)
        {
            TableNumber = tableNumber;
            Status = status;
            DrinkStatus = TableOrderStatus.Unknown;
            FoodStatus = TableOrderStatus.Unknown;
        }

        public TableStatusViewModel(int tableNumber, TableStatusType status, TableOrderStatus drinkStatus, TableOrderStatus foodStatus)
        {
            TableNumber = tableNumber;
            Status = status;
            DrinkStatus = drinkStatus;
            FoodStatus = foodStatus;
        }
    }
}
