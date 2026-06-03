using shappo0.Enums;
using shappo0.Models;
using System.Collections.Generic;

namespace shappo0.ViewModels
{
    public class TableViewModel
    {
        public int TableNumber { get; set; }
        public bool Status { get; set; }
        public OrderStatus DrinkStatus { get; set; }
        public OrderStatus FoodStatus { get; set; }
        public List<Order> Orders { get; set; }

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

        public TableViewModel(int tableNumber, bool status, List<Order> orders)
        {
            TableNumber = tableNumber;
            Status = status;
            Orders = orders ?? new List<Order>();
        }
    }
}