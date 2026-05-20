using shappo0.Models;

namespace shappo0.ViewModels
{
  
        public class PaymentViewModel
        {
            public int TableId { get; set; }
            public List<OrderItem> Items { get; set; }
            public double TotalAmount { get; set; }
            public double VatLow { get; set; }
            public double VatHigh { get; set; }

        public PaymentViewModel()
        {
        }

        public PaymentViewModel(int tableId, List<OrderItem> items, double totalAmount, double vatLow, double vatHigh)
        {
            TableId = tableId;
            Items = items;
            TotalAmount = totalAmount;
            VatLow = vatLow;
            VatHigh = vatHigh;
        }
    }
 
}
