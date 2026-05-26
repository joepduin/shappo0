namespace shappo0.Models
{
    // Model voor de rekening (Bill)
    // Dit is een simpele POCO-klasse die de database-tabel BILL weerspiegelt.
    // We gebruiken gewone properties zodat de repository deze eenvoudig kan vullen en opslaan.
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

        public Bill(int billId, decimal subTotal, decimal total, decimal vatLow, decimal vatHigh, bool isPaid, int visitId)
        {
            BillId = billId;
            SubTotal = subTotal;
            Total = total;
            VatLow = vatLow;
            VatHigh = vatHigh;
            IsPaid = isPaid;
            VisitId = visitId;
        }
    }
}
