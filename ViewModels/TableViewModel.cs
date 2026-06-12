using shappo0.Enums;
using shappo0.Models;

namespace shappo0.ViewModels
{
    public class TableViewModel
    {
        public Table Table { get; set; }
        public OrderStatus DrinkStatus { get; set; }
        public OrderStatus FoodStatus { get; set; }
        public List<OrderViewModel> Orders { get; set; }

        public TableViewModel()
        {
        }

        public TableViewModel(Table table, OrderStatus drinkStatus, OrderStatus foodStatus)
        {
            Table = table;
            DrinkStatus = drinkStatus;
            FoodStatus = foodStatus;
            Orders = new List<OrderViewModel>();
        }

        public TableViewModel(Table table, List<OrderViewModel> orders)
        {
            Table = table;
            Orders = orders ?? new List<OrderViewModel>();
        }
    }
}