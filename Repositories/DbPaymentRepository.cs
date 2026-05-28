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
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "INSERT INTO PAYMENT (paymentMethod, paymentAmount, billId, paymentTip) VALUES (@paymentMethod, @paymentAmount, @billId, @paymentTip); SELECT SCOPE_IDENTITY();";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@paymentMethod", payment.PaymentMethod.ToString());
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
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "INSERT INTO FEEDBACK ([type], description, paymentId) VALUES (@type, @description, @paymentId);";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@type", feedback.Type.ToString());
                    command.Parameters.AddWithValue("@description", feedback.Description);
                    command.Parameters.AddWithValue("@paymentId", feedback.PaymentId);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

    }
}
