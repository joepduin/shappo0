using shappo0.Enums;

namespace shappo0.Models
{
    public class Payment
    {
        public int PaymentId { get; set; }
        public PaymentMethodType PaymentMethod { get; set; }
        public decimal PaymentAmount { get; set; }
        public int BillId { get; set; }
        public decimal PaymentTip { get; set; }

        public Payment()
        {
        }

        public Payment(int paymentId, PaymentMethodType paymentMethod, decimal paymentAmount, int billId, decimal paymentTip)
        {
            PaymentId = paymentId;
            PaymentMethod = paymentMethod;
            PaymentAmount = paymentAmount;
            BillId = billId;
            PaymentTip = paymentTip;
        }
    }
}
