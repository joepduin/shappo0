using shappo0.Models;

namespace shappo0.ViewModels
{
    public class FinanceViewModel
    {
        public List<Finance> IncomeData { get; set; }
        public string? StartDate { get; set; }
        public string? EndDate { get; set; }
        public decimal TotalTips { get; set; }

        public FinanceViewModel(List<Finance> incomeData, DateRange? range, decimal totalTips)
        {
            IncomeData = incomeData;
            // Uses the ToString() method to avoid having to cast it in the View itself
            StartDate = range?.Start?.ToString("yyyy-MM-dd");
            EndDate = range?.End?.ToString("yyyy-MM-dd");
            TotalTips = totalTips;
        }
    }
}
