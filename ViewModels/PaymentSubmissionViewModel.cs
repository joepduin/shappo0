using shappo0.Enums;

namespace shappo0.ViewModels
{
    public class PaymentSubmissionViewModel
    {
        public int TableNumber { get; set; }
        public int VisitId { get; set; }
        public bool IsEqualSplit { get; set; }
        public int SplitCount { get; set; }
        public decimal TipAmount { get; set; }
        public decimal PaymentAmount { get; set; }
        public PaymentMethodType PaymentMethod { get; set; }
        public FeedbackCategory FeedbackType { get; set; }
        public string FeedbackDescription { get; set; }

        public PaymentSubmissionViewModel()
        {
        }

        public PaymentSubmissionViewModel(int tableNumber, int visitId, bool isEqualSplit, int splitCount, decimal tipAmount, decimal paymentAmount, PaymentMethodType paymentMethod, FeedbackCategory feedbackType, string feedbackDescription)
        {
            TableNumber = tableNumber;
            VisitId = visitId;
            IsEqualSplit = isEqualSplit;
            SplitCount = splitCount;
            TipAmount = tipAmount;
            PaymentAmount = paymentAmount;
            PaymentMethod = paymentMethod;
            FeedbackType = feedbackType;
            FeedbackDescription = feedbackDescription;
        }
    }
}
