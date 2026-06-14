using shappo0.Enums;
using shappo0.Models;

namespace shappo0.ViewModels
{
    public class TableViewModel
    {
        public Table Table { get; set; }
        public OrderStatus DrinkStatus { get; set; }
        public OrderStatus FoodStatus { get; set; }
        public List<Order> Orders { get; set; }
        public bool CanPay { get; set; }

        public TableViewModel()
        {
            Table = new Table();
            Orders = new List<Order>();
        }

        public TableViewModel(Table table, OrderStatus drinkStatus, OrderStatus foodStatus)
        {
            Table = table;
            DrinkStatus = drinkStatus;
            FoodStatus = foodStatus;
            Orders = new List<Order>();
        }

        public TableViewModel(Table table, List<Order> orders, bool canPay)
        {
            Table = table;
            Orders = orders;
            CanPay = canPay;
        }
    }
}