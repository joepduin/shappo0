using shappo0.Enums;
using shappo0.Models;

namespace shappo0.ViewModels
{
    public class TableViewModel
    {
        public Table Table { get; set; }
        public OrderStatus DrinkStatus { get; set; }
        public OrderStatus FoodStatus { get; set; }
        public List<OrderViewmodel> Orders { get; set; }

        public TableViewModel()
        {
        }

        public TableViewModel(Table table, OrderStatus drinkStatus, OrderStatus foodStatus)
        {
            Table = table;
            DrinkStatus = drinkStatus;
            FoodStatus = foodStatus;
            Orders = new List<OrderViewmodel>();
        }

        public TableViewModel(Table table, List<OrderViewmodel> orders)
        {
            Table = table;
            Orders = orders ?? new List<OrderViewmodel>();
        }
    }
}