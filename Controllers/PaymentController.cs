using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using shappo0.Services;
using shappo0.ViewModels;

namespace shappo0.Controllers
{
    public class PaymentController : Controller
    {
        // Controller voor betalingen
        // Deze controller vraagt het payment overzicht op bij de service
        // en geeft het door aan de view.
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            // Dependency injection: de service wordt ingevoerd via de constructor
            _paymentService = paymentService;
        }
        [Authorize]
        [HttpGet]
        public IActionResult Index(int tableId)
        {
            // Haal het betalings-overzicht op voor de opgegeven tafel
            PaymentViewModel model = _paymentService.GetPaymentOverview(tableId);
            // Geef het model door aan de view om te tonen
            return View(model);
        }
    }
}
