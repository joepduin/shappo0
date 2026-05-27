using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using shappo0.Services;
using shappo0.ViewModels;

namespace shappo0.Controllers
{
    public class PaymentController : Controller
    {
        // Controller voor betalingen
        // Uitleg (Nederlands):
        // Deze controller gebruikt Dependency Injection om de PaymentService te krijgen.
        // Dependency Injection betekent dat de controller geen nieuwe service maakt,
        // maar deze ontvangt via de constructor. Dat maakt testen en onderhoud makkelijker.
        // De controller bevat twee soorten acties:
        // - [HttpGet] acties tonen data (bijv. overzicht van de bestelling)
        // - [HttpPost] acties ontvangen formulierdata en voeren bewerkingen uit (bijv. betaling verwerken)
        // De model binding van ASP.NET koppelt automatisch form 'name' velden aan de parameters van de [HttpPost] methode.
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
            // GET acties tonen data: we vragen de service om een viewmodel en geven dat aan de view.
            PaymentViewModel model = _paymentService.GetPaymentOverview(tableId);
            // Geef het model door aan de view om te tonen
            return View(model);
        }

        [Authorize]
        [HttpPost]
        public IActionResult ProcessPayment(int tableId, int visitId, decimal subTotal, decimal vatLow, decimal vatHigh, decimal tipAmount, decimal paymentAmount, string paymentMethod, string feedbackType, string feedbackDescription)
        {
            // Uitleg: Model binding koppelt automatisch form velden met dezelfde 'name' aan deze parameters.
            // De controller ontvangt de form waarden en geeft ze door aan de service.
            // Roep service aan om de betaling te verwerken en op te slaan
            _paymentService.ProcessPayment(tableId, visitId, subTotal, vatLow, vatHigh, tipAmount, paymentAmount, paymentMethod, feedbackType, feedbackDescription);

            // Redirect naar de bevestigingspagina
            return RedirectToAction("PaymentSuccess", new { tableId = tableId });
        }

        [Authorize]
        [HttpGet]
        public IActionResult PaymentSuccess(int tableId)
        {
            // Eenvoudige bevestigingspagina. We zetten tableId in ViewBag zodat de view het kan tonen.
            ViewBag.TableId = tableId;
            return View();
        }
    }
}
