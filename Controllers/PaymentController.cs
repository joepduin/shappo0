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

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [Authorize]
        [HttpGet]
        public IActionResult Index(int tableNumber)
        {
            PaymentOverview overview = _paymentService.GetPaymentOverview(tableNumber);

            PaymentViewModel viewModelOverview = new PaymentViewModel
            {
                TableNumber = overview.TableNumber,
                VisitId = overview.VisitId,
                Items = overview.Items,
                TotalAmount = overview.TotalAmount,
                TotalPaid = overview.TotalPaid,
                RemainingAmount = overview.RemainingAmount,
                VatLow = overview.VatLow,
                VatHigh = overview.VatHigh
            };

            ViewData["PaymentOverview"] = viewModelOverview;

            PaymentSubmissionViewModel model = new PaymentSubmissionViewModel
            {
                TableNumber = tableNumber,
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
                PaymentOverview overview = _paymentService.GetPaymentOverview(submission.TableNumber);
                decimal expectedAmount = overview.TotalAmount / submission.SplitCount;

                if (submission.PaymentAmount < expectedAmount)
                {
                    ViewData["ErrorMessage"] = "When splitting equally, the payment amount must be at least the expected amount.";

                    PaymentViewModel viewModelOverview = new PaymentViewModel
                    {
                        TableNumber = overview.TableNumber,
                        VisitId = overview.VisitId,
                        Items = overview.Items,
                        TotalAmount = overview.TotalAmount,
                        TotalPaid = overview.TotalPaid,
                        RemainingAmount = overview.RemainingAmount,
                        VatLow = overview.VatLow,
                        VatHigh = overview.VatHigh
                    };

                    ViewData["PaymentOverview"] = viewModelOverview;
                    return View("Index", submission);
                }
            }

            PaymentRequest request = new PaymentRequest
            {
                TableNumber = submission.TableNumber,
                VisitId = submission.VisitId,
                IsEqualSplit = submission.IsEqualSplit,
                SplitCount = submission.SplitCount,
                PaymentAmount = submission.PaymentAmount,
                TipAmount = submission.TipAmount,
                PaymentMethod = submission.PaymentMethod,
                FeedbackType = submission.FeedbackType,
                FeedbackDescription = submission.FeedbackDescription
            };

            bool isFullyPaid = _paymentService.ProcessPayment(request);

            if (isFullyPaid)
            {
                return RedirectToAction("PaymentSuccess", new { tableNumber = submission.TableNumber });
            }

            return RedirectToAction("Index", new { tableNumber = submission.TableNumber });
        }

        [Authorize]
        [HttpGet]
        public IActionResult PaymentSuccess(int tableNumber)
        {
            bool isSuccess = _paymentService.VerifyPaymentSuccess(tableNumber);

            if (!isSuccess)
            {
                return RedirectToAction("Index", new { tableNumber = tableNumber });
            }

            ViewBag.TableNumber = tableNumber;
            return View();
        }
    }
}
