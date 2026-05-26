namespace shappo0.Models
{
    // Model voor feedback gekoppeld aan een betaling
    // Type en description geven commentaar van gasten weer
    public class Feedback
    {
        public int FeedbackId { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public int PaymentId { get; set; }

        public Feedback()
        {
        }

        public Feedback(int feedbackId, string type, string description, int paymentId)
        {
            FeedbackId = feedbackId;
            Type = type;
            Description = description;
            PaymentId = paymentId;
        }
    }
}
