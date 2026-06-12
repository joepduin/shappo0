using shappo0.Models;
using shappo0.ViewModels;

namespace shappo0.Services
{
    public interface IPaymentService
    {
        public Bill GenerateBillByVisit(Visit visit);
        public int SaveBill(Bill bill);
        public Bill? GetBillById(int? billId);
        public void SaveFeedback(Payment payment);
        public int SavePayment(Payment payment);
        public List<Payment> GetPaymentsByBillId(int billId);
        public decimal GetTotalPaidByBillId(int billId);
        public Bill? GetBillByVisitId(int visitId);
        public void UpdateBillPaid(Bill bill);
        public bool IsBillFullyPaid(Bill bill);
    }
}