using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using shappo0.Enums;
using shappo0.Models;
using shappo0.Services;
using shappo0.ViewModels;

namespace shappo0.Controllers
{
    [Authorize(Roles = "Waiter, Manager")]
    public class PaymentController : Controller
    {
        private readonly IPaymentService _paymentService;
        private readonly IVisitService _visitService;
        private readonly ITableService _tableService;

        public PaymentController(IPaymentService paymentService, IVisitService visitService, ITableService tableService)
        {
            _paymentService = paymentService;
            _visitService = visitService;
            _tableService = tableService;
        }

        [HttpGet]
        public IActionResult Index(int tableNumber)
        {
            //Haalt de visit op zodat dat de visitId gebruikt kan worden voor de GenerateBillByVisit methode.
            Visit? visit = _visitService.GetActiveVisitForTable(tableNumber);
            if (visit == null)
            {
                TempData["Error"] = $"No active visit found for table {tableNumber}.";
                return RedirectToAction("Index", "Table"); //null error
            }

            //Voodat de bill word gemaakt eerst even kijken of hij niet al gemaakt is.
            Bill? existingBill = _paymentService.GetBillByVisitId(visit.VisitId);
            //Als de bill wel al gemaakt is ga dan gelijk door naar afrekenen.
            if (existingBill != null) return RedirectToAction("ProcessPayment", new { billId = existingBill.BillId });

            //Genereerd de bill als er een actieve visit bij de tafel is.
            Bill bill = _paymentService.GenerateBillByVisit(visit);
            return View(bill);
        }


        [HttpPost]
        public ActionResult CreateBill(Bill bill)
        {
            //Slaat de bill op in de database en geeft BillId
            bill.BillId = _paymentService.SaveBill(bill);
            //Checkt of de betaling op een manier al is afgerond
            if (_paymentService.IsBillFullyPaid(bill)) return RedirectToAction("PaymentSuccess", new { visitId = bill.Visit.VisitId }); //suc6
            //Er is nog niet betaald dus IsPaid kan naar false
            return RedirectToAction("ProcessPayment", new { billId = bill.BillId });
        }

        [HttpPost]
        public ActionResult CreatePayment(Payment payment)
        {
            if (payment == null || payment?.Bill == null)
            {
                TempData["Error"] = "Invalid payment data submitted.";
                return RedirectToAction("Index", "Table"); //null error
            }

            Bill? bill = _paymentService.GetBillById(payment.Bill.BillId);
            if (bill == null)
            {
                TempData["Error"] = "The bill associated with this payment could not be found.";
                return RedirectToAction("Index", "Table"); //null error
            }

            //Dit hier pas doen want er kan geen ? bij de Bill in het payment object.
            payment.Bill = bill;
            //Slaat de payment op.
            payment.PaymentId = _paymentService.SavePayment(payment);
            //Slaat de feedback op als dat gegeven is.
            if (payment.Feedback?.Description != null) _paymentService.SaveFeedback(payment);
            //Checkt of de gehele bill is betaald, als dat zo is Redirect.
            if (_paymentService.IsBillFullyPaid(bill)) return RedirectToAction("PaymentSuccess", new { visitId = bill.Visit.VisitId }); //suc6
            //Als de gehele bill niet is betaald vraagt het voor een nieuwe betalling.
            return RedirectToAction("ProcessPayment", new { billId = payment.Bill.BillId });
        }

        [HttpGet]
        public IActionResult ProcessPayment(int billId)
        {
            //Haalt de bill op zodat die gegevens kunnen gebruikt worden voor het ViewModel.
            Bill? bill = _paymentService.GetBillById(billId);
            if (bill == null)
            {
                TempData["Error"] = "The bill associated with this payment could not be found.";
                return RedirectToAction("Index", "Table"); //null error
            }

            //Checkt of de bill al betaald is
            if (_paymentService.IsBillFullyPaid(bill)) return RedirectToAction("PaymentSuccess", new { visitId = bill.Visit.VisitId }); //suc6
            //Vult de visit in zodat tablenumber in het ViewModel kan.
            Visit? visit = _visitService.GetById(bill.Visit.VisitId);
            if (visit == null)
            {
                TempData["Error"] = "The visit associated with this bill could not be found.";
                return RedirectToAction("Index", "Table"); //null error
            }

            //Dit pas hier want anders werkt de ? niet.
            bill.Visit = visit;
            //Maakt het viewmodel met de CreatePaymentViewModel methode.
            PaymentViewModel paymentViewModel = CreatePaymentViewModel(bill);
            return View(paymentViewModel);
        }

        [HttpGet]
        public IActionResult PaymentSuccess(int visitId)
        {
            Bill? bill = _paymentService.GetBillByVisitId(visitId);
            if (bill == null)
            {
                TempData["Error"] = "The bill associated with this visit could not be found.";
                return RedirectToAction("Index", "Table"); //null error
            }

            Visit? visit = _visitService.GetById(visitId);
            if (visit == null)
            {
                TempData["Error"] = "The visit associated with this bill could not be found.";
                return RedirectToAction("Index", "Table"); //null error
            }

            visit.Table.Status = false;
            _visitService.CloseVisit(visitId);
            _tableService.UpdateTableStatus(visit.Table);
            List<Payment> payments = _paymentService.GetPaymentsByBillId(bill.BillId);
            bill.TotalTip = payments.Sum(p => p.PaymentTip);
            _paymentService.UpdateBillPaid(bill);
            return View();
        }

        //Ik moet nog even nadenken hoe ik dit aanpak, want een viewmodel mag niet in de services gemaakt worden en hier mogen geen berekeningen
        private PaymentViewModel CreatePaymentViewModel(Bill bill)
        {
            List<Payment> payments = _paymentService.GetPaymentsByBillId(bill.BillId);

            decimal amountToPay = 0;
            decimal paymentTip = 0;
            int numberOfPeople = 0;

            if (bill.SplitChoice == SplitChoice.Equal && bill.NumberOfPeople != null)
            {
                amountToPay = bill.Total / (decimal)bill.NumberOfPeople;
                paymentTip = bill.TotalTip / (decimal)bill.NumberOfPeople;
                numberOfPeople = (int)bill.NumberOfPeople;
            }
            else
            {
                amountToPay = bill.Total;
                paymentTip = bill.TotalTip;
                if (bill.SplitChoice == SplitChoice.Single) numberOfPeople = 1;
                else numberOfPeople = 0;  //er is nu geen idee wat de number numberOfPeople is, aangezien dat later pas word ingesteld. 
            }

            return new PaymentViewModel
            {
                BillId = bill.BillId,
                TableNumber = bill.Visit.Table.TableNumber,
                NumberOfPeoplePayed = payments.Count() + 1,
                NumberOfPeople = numberOfPeople,
                AmountToPay = amountToPay,
                OutstandingAmount = bill.Total - _paymentService.GetTotalPaidByBillId(bill.BillId),
                SplitChoice = bill.SplitChoice
            };
        }
    }
}