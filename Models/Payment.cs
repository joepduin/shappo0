using shappo0.Enums;

namespace shappo0.Models
{
    public class Payment
    {
        public int PaymentId { get; set; }
        public PaymentMethodType PaymentMethod { get; set; }
        public decimal PaymentAmount { get; set; }
        public Bill Bill { get; set; }
        public decimal PaymentTip { get; set; }
        public Feedback? Feedback { get; set; }

        public Payment()
        {
            Bill = new();
            Feedback = new();
        }

        public Payment(int paymentId, PaymentMethodType paymentMethod, decimal paymentAmount, Bill bill, decimal paymentTip, Feedback? feedback)
        {
            PaymentId = paymentId;
            PaymentMethod = paymentMethod;
            PaymentAmount = paymentAmount;
            Bill = bill;
            PaymentTip = paymentTip;
            Feedback = feedback;
        }
    }
}
