using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using shappo0.Models;
using shappo0.Services.Interfaces;
using shappo0.ViewModels;

namespace shappo0.Controllers
{
    [Authorize(Roles = "Manager")]
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
            // Sends null values to the helper method in order to activate the default date range (30 days) in the service
            return ProcessFinanceView(null, null);
        }

        [HttpPost]
        public IActionResult Index(DateRange? range, string quickFilter)
        {
            // Sends along the user-selected values to the helper method
            return ProcessFinanceView(range, quickFilter);
        }

        // Helper method for the GET and POST methods to prevent double code
        private IActionResult ProcessFinanceView(DateRange? range, string? filter)
        {
            // Retrieves the income data and total tips based on the provided date range and quick filter
            List<Finance> incomeData = _financeService.GetIncomeByDate(range, filter);
            decimal totalTips = _financeService.GetTipsByDate(range, filter);
            // Retrieves the selected range so it can be used in the view
            DateRange? resolvedRange = _financeService.CalculateRange(range, filter);

            // Creates ViewModel: start & end date, total tips and a list with the income data
            FinanceViewModel viewModel = new FinanceViewModel(incomeData, resolvedRange, totalTips);

            return View(viewModel);
        }
    }
}