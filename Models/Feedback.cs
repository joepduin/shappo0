using shappo0.Enums;

namespace shappo0.Models
{
    public class Feedback
    {
        public int FeedbackId { get; set; }
        public FeedbackType Type { get; set; }
        public string Description { get; set; }

        public Feedback()
        {

        }

        public Feedback(int feedbackId, FeedbackType type, string description)
        {
            FeedbackId = feedbackId;
            Type = type;
            Description = description;
        }
    }
}
