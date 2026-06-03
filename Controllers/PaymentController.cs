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
        private readonly IPaymentRepository _paymentRepository;

        public PaymentController(IPaymentService paymentService, IVisitRepository visitRepository, IPaymentRepository paymentRepository)
        {
            _paymentService = paymentService;
            _visitRepository = visitRepository;
            _paymentRepository = paymentRepository;
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
            if (submission.IsEqualSplit && submission.SplitCount > 0)
            {
                PaymentViewModel overview = _paymentService.GetPaymentOverview(submission.TableId);
                decimal expectedAmount = overview.TotalAmount / submission.SplitCount;

                if (submission.PaymentAmount < expectedAmount)
                {
                    ModelState.AddModelError("PaymentAmount", "Bij gelijk verdelen moet het bedrag minimaal het verwachte bedrag zijn.");
                    ViewBag.PaymentOverview = overview;
                    return View("Index", submission);
                }
            }

            _paymentService.ProcessPayment(submission);
            Bill bill = _paymentRepository.GetBillByVisitId(submission.VisitId);
            if (bill != null)
            {
                decimal totalPaid = _paymentRepository.GetTotalPaidForBill(bill.BillId);
                if (totalPaid >= bill.Total)
                    return RedirectToAction("PaymentSuccess", new { tableId = submission.TableId });
            }

            return RedirectToAction("Index", new { tableId = submission.TableId });
        }

        [Authorize]
        [HttpGet]
        public IActionResult PaymentSuccess(int tableId)
        {
            Visit activeVisit = _visitRepository.GetActiveVisitForTable(tableId);

            if (activeVisit != null)
            {
                Bill bill = _paymentRepository.GetBillByVisitId(activeVisit.VisitId);
                if (bill == null || bill.IsPaid == false)
                    return RedirectToAction("Index", new { tableId = tableId });
            }

            ViewBag.TableId = tableId;
            return View();
        }
    }
}
