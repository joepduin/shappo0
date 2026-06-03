using shappo0.Models;

namespace shappo0.ViewModels
{
  
        public class PaymentViewModel
        {
            public int TableNumber { get; set; }
            public int VisitId { get; set; }
            public List<OrderLine> Items { get; set; }
            public decimal TotalAmount { get; set; }
        public decimal TotalPaid { get; set; }
        public decimal RemainingAmount { get; set; }
            public decimal VatLow { get; set; }
            public decimal VatHigh { get; set; }

        public PaymentViewModel()
        {
        }

        public PaymentViewModel(int tableNumber, List<OrderLine> items, decimal totalAmount, decimal vatLow, decimal vatHigh)
        {
            TableNumber = tableNumber;
            Items = items;
            TotalAmount = totalAmount;
            VatLow = vatLow;
            VatHigh = vatHigh;
        }
    }
 
}
