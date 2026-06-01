using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using shappo0.Models;
using shappo0.Services;
using shappo0.Services.Interfaces;

namespace shappo0.Controllers
{
    [Authorize (Roles = "Manager")]
    public class FinanceController : Controller
    {
        private readonly IFinanceService _financeService;

        public FinanceController(IFinanceService financeService)
        {
            _financeService = financeService;
        }
        [HttpGet]
        public IActionResult Index()
        {
            // Default to the last 30 days on initial load
            DateTime startDate = DateTime.Today.AddDays(-30);
            DateTime endDate = DateTime.Today;

            List<Finance> incomeData = _financeService.GetIncomeByDate(startDate, endDate);
            decimal totalTips = _financeService.GetTipsByDate(startDate, endDate);

            ViewBag.StartDate = startDate.ToString("yyyy-MM-dd");
            ViewBag.EndDate = endDate.ToString("yyyy-MM-dd");
            ViewBag.TotalTips = totalTips;

            return View(incomeData);
        }

        [HttpPost]
        public IActionResult Index(DateTime? startDate, DateTime? endDate, string quickFilter)
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
                    finalStart = startDate ?? today.AddDays(-30);
                    finalEnd = endDate ?? today;
                    break;
            }

            // gets the records from the database
            List<Finance> incomeData = _financeService.GetIncomeByDate(finalStart, finalEnd);
            decimal totalTips = _financeService.GetTipsByDate(finalStart, finalEnd);

            // Adds to viewbag so that the view displays the selected dates & tips
            ViewBag.StartDate = finalStart.ToString("yyyy-MM-dd");
            ViewBag.EndDate = finalEnd.ToString("yyyy-MM-dd");
            ViewBag.TotalTips = totalTips;

            return View(incomeData);
        }
    }
}
