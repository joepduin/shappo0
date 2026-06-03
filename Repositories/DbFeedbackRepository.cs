using shappo0.Models;
using Microsoft.Data.SqlClient;
using shappo0.Repositories.Interfaces;

namespace shappo0.Repositories
{
    public class DbFeedbackRepository : IFeedbackRepository
    {
        private readonly string? _connectionString;

        public DbFeedbackRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("shappo0");
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
