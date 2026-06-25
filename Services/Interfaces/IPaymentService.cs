using shappo0.Enums;
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
        public Bill? GetBillByVisitId(int visitId);
        public void UpdateBillPaid(Bill bill);
        public bool IsBillFullyPaid(Bill bill);
        public Payment CalculatePaymentDetails(Bill bill);
        public int CalculateNumberOfPeoplePaid(Bill bill);
        public decimal CalculateOutstandingAmount(Bill bill);
        public decimal CalculateTotalTip(Bill bill);
        public Bill? GetPaidBill(int visitId);
    }
}