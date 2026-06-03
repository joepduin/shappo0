using shappo0.Models;
using shappo0.ViewModels;

namespace shappo0.Services
{
    public interface IPaymentService
    {
        PaymentOverview GetPaymentOverview(int tableNumber);
        bool ProcessPayment(PaymentRequest request);
        bool VerifyPaymentSuccess(int tableNumber);
    }
}
