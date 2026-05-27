using shappo0.ViewModels;

namespace shappo0.Services
{
    public interface IPaymentService
    {
        PaymentViewModel GetPaymentOverview(int tableId);
        // Service interface voor betalingen
        // Uitleg: de service bevat de business logic en orkestreert repositories.
        // ProcessPayment verwerkt en slaat de volledige betaling op (bill, payment, feedback)
        // paymentAmount is optioneel: als ingevuld door gast wordt dat gebruikt als betaalde totaal.
        void ProcessPayment(int tableId, int visitId, decimal subTotal, decimal vatLow, decimal vatHigh, decimal tipAmount, decimal paymentAmount, string paymentMethod, string feedbackType, string feedbackDescription);
    }
}
