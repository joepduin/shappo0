using shappo0.Enums;

namespace shappo0.Extensions
{
    public static class EmojiExtensions
    {
        public static string ToEmoji(this OrderStatus orderStatus)
        {
            if (orderStatus == OrderStatus.Ordered) return "🧾";
            if (orderStatus == OrderStatus.BeingPrepared) return "👨‍🍳";
            if (orderStatus == OrderStatus.ReadyToBeServed) return "🍽️";
            else return ""; //ik weet niet welke emoji ik hier moet doen als er geen actieve order is.
        }
    }
}