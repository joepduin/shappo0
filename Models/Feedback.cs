using shappo0.Enums;

namespace shappo0.Models
{
    public class Feedback
    {
        public int FeedbackId { get; set; }
        public FeedbackCategory Type { get; set; }
        public string Description { get; set; }
        public int PaymentId { get; set; }

        public Feedback()
        {
        }

        public Feedback(int feedbackId, FeedbackCategory type, string description, int paymentId)
        {
            FeedbackId = feedbackId;
            Type = type;
            Description = description;
            PaymentId = paymentId;
        }
    }
}
