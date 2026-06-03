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
        public int VisitId { get; set; }

        public Bill()
        {
        }

        public Bill(decimal subTotal, decimal total, decimal vatLow, decimal vatHigh, bool isPaid, int visitId)
        {
            SubTotal = subTotal;
            Total = total;
            VatLow = vatLow;
            VatHigh = vatHigh;
            IsPaid = isPaid;
            VisitId = visitId;
        }
    }
}
