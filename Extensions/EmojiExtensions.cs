using shappo0.Enums;

namespace shappo0.Extensions
{
    public static class EmojiExtensions
    {
        //Zet de enum om naar emoji voor het tafel overzicht.
        public static string ToEmoji(this OrderStatus orderStatus)
        {
            switch (orderStatus)
            {
                case OrderStatus.Ordered:
                    return "🧾";
                case OrderStatus.BeingPrepared:
                    return "👨‍🍳";
                case OrderStatus.ReadyToBeServed:
                    return "🍽️";
                default:
                    return "💤";
            }
        }
    }
}
