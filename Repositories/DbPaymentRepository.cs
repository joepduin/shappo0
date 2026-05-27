using Microsoft.Data.SqlClient;
using shappo0.Models;
using Microsoft.Extensions.Configuration;

namespace shappo0.Repositories
{
    public class DbPaymentRepository : IPaymentRepository
    {
        private readonly string? _connectionString;

        public DbPaymentRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("shappo0");
        }

        public int SaveBill(Bill bill)
        {
            // Uitleg (Nederlands):
            // We gebruiken parameterized queries (AddWithValue) om SQL-injectie te voorkomen.
            // ExecuteScalar retourneert de eerste kolom van de eerste rij; bij INSERT met SCOPE_IDENTITY
            // gebruiken we dit om de nieuw aangemaakte identity (billId) terug te krijgen.
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "INSERT INTO BILL (subTotal, total, VATLow, VATHigh, isPaid, visitId) VALUES (@subTotal, @total, @vatLow, @vatHigh, @isPaid, @visitId); SELECT SCOPE_IDENTITY();";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@subTotal", bill.SubTotal);
                    command.Parameters.AddWithValue("@total", bill.Total);
                    command.Parameters.AddWithValue("@vatLow", bill.VatLow);
                    command.Parameters.AddWithValue("@vatHigh", bill.VatHigh);
                    command.Parameters.AddWithValue("@isPaid", bill.IsPaid);
                    command.Parameters.AddWithValue("@visitId", bill.VisitId);

                    connection.Open();
                    return Convert.ToInt32(command.ExecuteScalar());
                }
            }
        }

        public int SavePayment(Payment payment)
        {
            // Uitleg: zelfde reden als SaveBill: we gebruiken parameterized queries
            // om te beschermen tegen SQL-injectie. ExecuteScalar haalt de nieuwe paymentId op.
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "INSERT INTO PAYMENT (paymentMethod, paymentAmount, billId, paymentTip) VALUES (@paymentMethod, @paymentAmount, @billId, @paymentTip); SELECT SCOPE_IDENTITY();";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@paymentMethod", payment.PaymentMethod);
                    command.Parameters.AddWithValue("@paymentAmount", payment.PaymentAmount);
                    command.Parameters.AddWithValue("@billId", payment.BillId);
                    command.Parameters.AddWithValue("@paymentTip", payment.PaymentTip);

                    connection.Open();
                    return Convert.ToInt32(command.ExecuteScalar());
                }
            }
        }

        public void SaveFeedback(Feedback feedback)
        {
            // Uitleg: feedback bevat tekst van gasten. Ook hier parameterized query om veiligheid te garanderen.
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "INSERT INTO FEEDBACK ([type], description, paymentId) VALUES (@type, @description, @paymentId);";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@type", feedback.Type);
                    command.Parameters.AddWithValue("@description", feedback.Description);
                    command.Parameters.AddWithValue("@paymentId", feedback.PaymentId);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public void FreeTable(int tableNumber)
        {
            // Uitleg: we werken direct op de [TABLE] tabel en zetten status op 0 (vrij)
            // Parameterized query voorkomt SQL-injectie.
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "UPDATE [TABLE] SET [status] = 0 WHERE tableNumber = @tableNumber";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@tableNumber", tableNumber);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
