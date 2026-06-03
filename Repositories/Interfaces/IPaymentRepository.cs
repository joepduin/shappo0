using shappo0.Models;

namespace shappo0.Repositories
{
    public interface IPaymentRepository
    {
        int SavePayment(Payment payment);
        decimal GetTotalPaidForBill(int billId);
    }
}
