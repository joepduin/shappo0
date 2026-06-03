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

        public decimal GetTotalPaidForBill(int billId)
        {
            decimal totalPaid = 0;
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT paymentAmount FROM PAYMENT WHERE billId = @billId";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@billId", billId);

                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            totalPaid += (decimal)reader["paymentAmount"];
                        }
                    }
                }
            }
            return totalPaid;
        }
    }
}