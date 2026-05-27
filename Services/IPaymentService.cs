using shappo0.ViewModels;

namespace shappo0.Services
{
    public interface IPaymentService
    {
        PaymentViewModel GetPaymentOverview(int tableId);
    }
}
