using shappo0.Models;

namespace shappo0.ViewModels
{
    public class TakeOrderViewModel
    {
        public int TableNumber { get; set; }
        public List<Product> AllProducts  { get; set; }
        public List<OrderLine> OrderLines { get; set; }

        public TakeOrderViewModel(int tableNumber)
        {
            TableNumber = tableNumber;
            AllProducts = new List<Product>();
            OrderLines = new List<OrderLine>();
        }
    }
}
