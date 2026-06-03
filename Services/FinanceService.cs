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

        public List<Finance> GetIncomeByDate(DateTime? startDate, DateTime? endDate, string? quickFilter)
        {
            DateRange range = CalculateRange(startDate, endDate, quickFilter);
            return _financeRepository.GetIncomeByDate(range.Start, range.End);
        }

        public decimal GetTipsByDate(DateTime? startDate, DateTime? endDate, string? quickFilter)
        {
            DateRange range = CalculateRange(startDate, endDate, quickFilter);
            return _financeRepository.GetTipsByDate(range.Start, range.End);
        }

        public DateRange CalculateRange(DateTime? startDate, DateTime? endDate, string? quickFilter)
        {
            DateTime today = DateTime.Today;
            DateTime finalStart;
            DateTime finalEnd = today;

            switch (quickFilter?.ToLower())
            {
                case "month":
                    finalStart = new DateTime(today.Year, today.Month, 1);
                    break;
                case "quarter":
                    int quarterStartMonth = ((today.Month - 1) / 3) * 3 + 1;
                    finalStart = new DateTime(today.Year, quarterStartMonth, 1);
                    break;
                case "year":
                    finalStart = new DateTime(today.Year, 1, 1);
                    break;
                default:
                    finalStart = startDate ?? today.AddDays(-30);
                    finalEnd = endDate ?? today;
                    break;
            }

            // Instantiating and returning the object explicitly
            DateRange range = new DateRange();
            range.Start = finalStart;
            range.End = finalEnd;

            return range;
        }
    }
}