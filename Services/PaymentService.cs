using shappo0.Enums;
using shappo0.Models;
using shappo0.Repositories;

namespace shappo0.Services
{
    public class PaymentService : IPaymentService
    {
        private const int VAT_LOW = 9;
        private const int VAT_HIGH = 21;

        private readonly IPaymentRepository _paymentRepository;
        private readonly IOrderService _orderService;

        public PaymentService(IPaymentRepository paymentRepository, IOrderService orderService)
        {
            _paymentRepository = paymentRepository;
            _orderService = orderService;
        }
        public Bill GenerateBillByVisit(Visit visit)
        {
            List<OrderLine> orderLines = _orderService.GetGroupedOrderLinesByVisitId(visit.VisitId);
            decimal subTotal = 0, total = 0, vatLow = 0, vatHigh = 0;

            foreach (OrderLine orderLine in orderLines)
            {
                decimal orderLineSubTotal = orderLine.Product.Price * orderLine.Quantity;
                subTotal += orderLineSubTotal;
                decimal orderLineTotal = orderLine.Product.PriceIncludingTax * orderLine.Quantity;
                total += orderLineTotal;
                decimal vatAmount = orderLineTotal - orderLineSubTotal;

                if (orderLine.Product.VATPercentage == VAT_LOW)
                {
                    vatLow += vatAmount;
                }
                else if (orderLine.Product.VATPercentage == VAT_HIGH)
                {
                    vatHigh += vatAmount;
                }
            }
            return new Bill { SubTotal = subTotal, Total = total, VatLow = vatLow, VatHigh = vatHigh, IsPaid = false, Visit = visit, OrderLines = orderLines };
        }

        public Bill? GetBillById(int? billId)
        {
            return _paymentRepository.GetBillById(billId);
        }

        public int SaveBill(Bill bill)
        {
            return _paymentRepository.SaveBill(bill);
        }

        public void SaveFeedback(Payment payment)
        {
            _paymentRepository.SaveFeedback(payment);
        }

        public int SavePayment(Payment payment)
        {
            return _paymentRepository.SavePayment(payment);
        }

        public Bill? GetBillByVisitId(int visitId)
        {
            return _paymentRepository.GetBillByVisitId(visitId);
        }

        public void UpdateBillPaid(Bill bill)
        {
            _paymentRepository.UpdateBillPaid(bill);
        }

        public bool IsBillFullyPaid(Bill bill)
        {
            //Er kunnen afrondings problemen zijn daarom gebruiken we >=, bijvoorbeeld € 50,00 / 3
            if (_paymentRepository.GetTotalPaidByBillId(bill.BillId) >= bill.Total) return true;
            return false;
        }

        public Payment CalculatePaymentDetails(Bill bill)
        {
            Payment payment = new Payment
            {
                Bill = new Bill
                {
                    BillId = bill.BillId,
                    Visit = bill.Visit,
                    SplitChoice = bill.SplitChoice,
                    NumberOfPeople = GetTotalPeople(bill)
                }
            };

            if (bill.SplitChoice == SplitChoice.Equal && bill.NumberOfPeople != null)
            {
                payment.PaymentAmount = bill.Total / bill.NumberOfPeople.Value;
                payment.PaymentTip = bill.TotalTip / bill.NumberOfPeople.Value;
            }
            else
            {
                payment.PaymentAmount = bill.Total;
                payment.PaymentTip = bill.TotalTip;
            }

            return payment;
        }

        private int GetTotalPeople(Bill bill)
        {
            if (bill.SplitChoice == SplitChoice.Equal && bill.NumberOfPeople != null) return bill.NumberOfPeople.Value;
            else if (bill.SplitChoice == SplitChoice.Single) return 1;
            else return 0;
        }

        public Bill? GetPaidBill(int visitId)
        {
            Bill? bill = _paymentRepository.GetBillByVisitId(visitId);
            List<OrderLine>? OrderLines = _orderService.GetGroupedOrderLinesByVisitId(visitId);

            if (OrderLines != null || bill != null) bill.OrderLines = OrderLines;

            return bill;
        }


        public int CalculateNumberOfPeoplePaid(Bill bill)
        {
            List<Payment> payments = GetPaymentsByBill(bill);
            return payments.Count();
        }

        public decimal CalculateOutstandingAmount(Bill bill)
        {
            decimal paidAmount = _paymentRepository.GetTotalPaidByBillId(bill.BillId);
            return bill.Total - paidAmount;
        }

        public decimal CalculateTotalTip(Bill bill)
        {
            List<Payment> payments = GetPaymentsByBill(bill);
            return payments.Sum(p => p.PaymentTip); //linq
        }

        private List<Payment> GetPaymentsByBill(Bill bill)
        {
            return _paymentRepository.GetPaymentsByBillId(bill.BillId);
        }
    }
}
