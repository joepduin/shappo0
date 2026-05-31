using shappo0.Models;

namespace shappo0.Repositories
{
    public interface IPaymentRepository
    {
        int SaveBill(Bill bill);
        int SavePayment(Payment payment);
        void SaveFeedback(Feedback feedback);
    }
}
