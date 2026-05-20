using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using shappo0.Services;
using shappo0.ViewModels;

namespace shappo0.Controllers
{
    public class PaymentController : Controller
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }
        [Authorize]
        [HttpGet]
        public IActionResult Index(int tableId)
        {
            PaymentViewModel model = _paymentService.GetPaymentOverview(tableId);
            return View(model);
        }
    }
}
