namespace shappo0.Models
{
    // Model voor een betaling (Payment)
    // Slaat informatie per betaalactie op: methode, bedrag, fooi en link naar Bill
    public class Payment
    {
        public int PaymentId { get; set; }
        public string PaymentMethod { get; set; }
        public decimal PaymentAmount { get; set; }
        public int BillId { get; set; }
        public decimal PaymentTip { get; set; }

        public Payment()
        {
        }

        public Payment(int paymentId, string paymentMethod, decimal paymentAmount, int billId, decimal paymentTip)
        {
            PaymentId = paymentId;
            PaymentMethod = paymentMethod;
            PaymentAmount = paymentAmount;
            BillId = billId;
            PaymentTip = paymentTip;
        }
    }
}
