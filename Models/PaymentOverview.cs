namespace shappo0.Models
{
    public class PaymentOverview
    {
        public int TableNumber { get; set; }
        public int VisitId { get; set; }
        public List<OrderLine> Items { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal TotalPaid { get; set; }
        public decimal RemainingAmount { get; set; }
        public decimal VatLow { get; set; }
        public decimal VatHigh { get; set; }
    }
}
