using shappo0.Models;

namespace shappo0.Repositories
{
    public interface IPaymentRepository
    {
        int SaveBill(Bill bill);
        int SavePayment(Payment payment);
        void SaveFeedback(Feedback feedback);
        Bill GetBillByVisitId(int visitId);
        decimal GetTotalPaidForBill(int billId);
        void UpdateBillPaid(int billId, bool isPaid);
    }
}
