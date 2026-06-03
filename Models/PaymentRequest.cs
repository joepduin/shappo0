using shappo0.Enums;
namespace shappo0.Models
    
{
    public class PaymentRequest
    {
        public int TableNumber { get; set; }
        public int VisitId { get; set; }
        public bool IsEqualSplit { get; set; }
        public int SplitCount { get; set; }
        public decimal PaymentAmount { get; set; }
        public decimal TipAmount { get; set; }
        public PaymentMethodType PaymentMethod { get; set; }
        public FeedbackCategory FeedbackType { get; set; }
        public string FeedbackDescription { get; set; }
    }
}
