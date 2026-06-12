using shappo0.Models;

namespace shappo0.Repositories
{
    public interface IPaymentRepository
    {
        public int SavePayment(Payment payment);
        public decimal GetTotalPaidByBillId(int billId);
        public List<Payment> GetPaymentsByBillId(int billId);

        public Bill? GetBillByVisitId(int visitId);
        public int SaveBill(Bill bill);
        public Bill? GetBillByVisitId(int? visitId);
        public Bill? GetBillById(int? billId);
        public void UpdateBillPaid(Bill bill);

        public void SaveFeedback(Payment payment);
    }
}
