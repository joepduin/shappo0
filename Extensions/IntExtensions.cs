namespace shappo0.Extensions
{
    public static class IntExtensions
    {
        public static bool IsNegative(this int i)
        {
            return i < 0;
        }

        public static bool IsPositive(this int i)
        {
            return i > 0;
        }
    }
}
