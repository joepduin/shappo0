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
            return new Bill(subTotal, total, vatLow, vatHigh, false, visit, orderLines);
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

        public List<Payment> GetPaymentsByBillId(int billId)
        {
            return _paymentRepository.GetPaymentsByBillId(billId);
        }

        public decimal GetTotalPaidByBillId(int billId)
        {
            return _paymentRepository.GetTotalPaidByBillId(billId);
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
    }
}