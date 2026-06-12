using shappo0.Enums;
namespace shappo0.Models
{
    public class Bill
    {
        public int BillId { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Total { get; set; }
        public decimal VatLow { get; set; }
        public decimal VatHigh { get; set; }
        public bool IsPaid { get; set; }
        public decimal TotalTip { get; set; }
        public SplitChoice SplitChoice { get; set; }
        public int? NumberOfPeople { get; set; }
        public Visit Visit { get; set; }
        public List<OrderLine> OrderLines { get; set; }

        public Bill()
        {
            Visit = new();
            OrderLines = [];
        }

        public Bill(decimal subTotal, decimal total, decimal vatLow, decimal vatHigh, bool isPaid, decimal totalTip, SplitChoice splitChoice, int? numberOfPeople, Visit visit, List<OrderLine> orderLines)
        {
            SubTotal = subTotal;
            Total = total;
            VatLow = vatLow;
            VatHigh = vatHigh;
            IsPaid = isPaid;
            TotalTip = totalTip;
            Visit = visit;
            OrderLines = orderLines;
            SplitChoice = splitChoice;
            NumberOfPeople = numberOfPeople;
        }
    }
}
