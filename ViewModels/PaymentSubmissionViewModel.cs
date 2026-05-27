using shappo0.Enums;

namespace shappo0.ViewModels
{
    public class PaymentSubmissionViewModel
    {
        public int TableId { get; set; }
        public int VisitId { get; set; }
        public decimal TipAmount { get; set; }
        public decimal PaymentAmount { get; set; }
        public PaymentMethodType PaymentMethod { get; set; }
        public FeedbackCategory FeedbackType { get; set; }
        public string FeedbackDescription { get; set; }

        public PaymentSubmissionViewModel()
        {
        }

        public PaymentSubmissionViewModel(int tableId, int visitId, decimal tipAmount, decimal paymentAmount, PaymentMethodType paymentMethod, FeedbackCategory feedbackType, string feedbackDescription)
        {
            TableId = tableId;
            VisitId = visitId;
            TipAmount = tipAmount;
            PaymentAmount = paymentAmount;
            PaymentMethod = paymentMethod;
            FeedbackType = feedbackType;
            FeedbackDescription = feedbackDescription;
        }
    }
}
