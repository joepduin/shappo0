namespace shappo0.Models
{
    public class DateRange
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }

        public DateRange()
            { }
        public DateRange(DateTime start, DateTime end)
        {
            Start = start;
            End = end;
        }
    }
}
