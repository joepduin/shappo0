using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using shappo0.Models;
using shappo0.Repositories;
using shappo0.Services;
using shappo0.ViewModels;

namespace shappo0.Controllers
{
    public class PaymentController : Controller
    {
        private readonly IPaymentService _paymentService;
        private readonly IVisitRepository _visitRepository;

        public PaymentController(IPaymentService paymentService, IVisitRepository visitRepository)
        {
            _paymentService = paymentService;
            _visitRepository = visitRepository;
        }

        [Authorize]
        [HttpGet]
        public IActionResult Index(int tableId)
        {
            PaymentViewModel overview = _paymentService.GetPaymentOverview(tableId);
            ViewBag.PaymentOverview = overview;

            PaymentSubmissionViewModel model = new PaymentSubmissionViewModel
            {
                TableId = tableId,
                VisitId = overview.VisitId
            };

            return View(model);
        }

        [Authorize]
        [HttpPost]
        public IActionResult ProcessPayment(PaymentSubmissionViewModel submission)
        {
            _paymentService.ProcessPayment(submission);
            return RedirectToAction("PaymentSuccess", new { tableId = submission.TableId });
        }

        [Authorize]
        [HttpGet]
        public IActionResult PaymentSuccess(int tableId)
        {
            Visit activeVisit = _visitRepository.GetActiveVisitForTable(tableId);

            if (activeVisit != null)
            {
                return RedirectToAction("Index", new { tableId = tableId });
            }

            ViewBag.TableId = tableId;
            return View();
        }
    }
}
