using shappo0.Models;
using shappo0.Repositories.Interfaces;
using shappo0.Services.Interfaces;

namespace shappo0.Services
{
    public class FinanceService : IFinanceService
    {
        private readonly IFinanceRepository _financeRepository;

        public FinanceService(IFinanceRepository financeRepository)
        {
            _financeRepository = financeRepository;
        }

        // Uses a helper method to prevent code duplication
        public List<Finance> GetIncomeByDate(DateRange range, string? quickFilter)
        {
            DateRange calculatedRange = CalculateRange(range, quickFilter);
            return _financeRepository.GetIncomeByDate(calculatedRange);
        }

        public decimal GetTipsByDate(DateRange range, string? quickFilter)
        {
            DateRange calculatedRange = CalculateRange(range, quickFilter);
            return _financeRepository.GetTipsByDate(calculatedRange);
        }

        public DateRange CalculateRange(DateRange? range, string? quickFilter)
        {
            DateTime today = DateTime.Today;
            DateTime finalStart;
            DateTime finalEnd = today; // In case of custom selection this variable is overwritten later on, otherwise today is used 

            switch (quickFilter?.ToLower())
            {
                case "month":
                    // This selects the first day of the current month
                    finalStart = new DateTime(today.Year, today.Month, 1);
                    break;
                case "quarter":
                    // This selects the current month, then calculates which quarter it is in (1, 2, 3 or 4), then multiplies that by 3 to get the last month of the quarter, then adds 1 which will return to the standard 12 month scale
                    // The math behind this is kind of complicated, but this is what I found on the internet and it works and I understand it!
                    int quarterStartMonth = ((today.Month - 1) / 3) * 3 + 1;
                    finalStart = new DateTime(today.Year, quarterStartMonth, 1);
                    break;
                case "year":
                    // This selects the first day of the current year
                    finalStart = new DateTime(today.Year, 1, 1);
                    break;
                default:
                    // Defaults to 30 days if no data was provided, otherwise uses the selected dates
                    finalStart = range?.Start ?? today.AddDays(-30);
                    finalEnd = range?.End ?? today;
                    break;
            }

            // Initializing and returning the object
            DateRange result = new DateRange();
            result.Start = finalStart;
            result.End = finalEnd;

            return result;
        }
    }
}