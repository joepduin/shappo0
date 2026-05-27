using shappo0.Models;

namespace shappo0.Repositories
{
    // Repository interface voor betalingen
    // Deze interface definieert de database-operaties die nodig zijn om:
    // - een Bill op te slaan
    // - een Payment op te slaan
    // - feedback op te slaan
    // - een tafel vrij te zetten
    // Implementaties gebruiken ADO.NET met parameterized queries om veilig data te schrijven.
    public interface IPaymentRepository
    {
        // Slaat een Bill op en retourneert het nieuw gegenereerde BillId
        int SaveBill(Bill bill);

        // Slaat een Payment op en retourneert het nieuw gegenereerde PaymentId
        int SavePayment(Payment payment);

        // Slaat feedback gekoppeld aan een payment op
        void SaveFeedback(Feedback feedback);

        // Zet een tafel vrij door status in [TABLE] op 0 te zetten
        void FreeTable(int tableNumber);
    }
}
