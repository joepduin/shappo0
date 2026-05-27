using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using shappo0.Models;

namespace shappo0.Repositories
{
    public class DbOrderRepository : IOrderRepository
    {
        private readonly string? _connectionString;

        public DbOrderRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("shappo0");
        }

        public List<Order> GetAllOrders()
        {
            List<Order> orders = new List<Order>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT orderNumber, timeCreated, waitingTimeMinutes, status, visitId, staffId FROM [Order]";

                SqlCommand command = new SqlCommand(query, connection);

                command.Connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Order order = ReadOrder(reader);
                    orders.Add(order);
                }
                reader.Close();
            }
            return orders;
        }

        public List<Order> GetRunningOrders()
        {
            List<Order> orders = new List<Order>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT orderNumber, timeCreated, waitingTimeMinutes, status, visitId, staffId FROM [Order] " +
                    "WHERE [status] = 'running'";

                SqlCommand command = new SqlCommand(query, connection);

                command.Connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Order order = ReadOrder(reader);
                    orders.Add(order);
                }
                reader.Close();
            }
            return orders;
        }

        private Order ReadOrder(SqlDataReader reader)
        {
            int orderNumber = (int)reader["orderNumber"];
            DateTime timeCreated = (DateTime)reader["timeCreated"];
            int waitingTimeMinutes = (int)reader["waitingTimeMinutes"];

            string statusString = (string)reader["status"];
            OrderStatus status = Enum.Parse<OrderStatus>(statusString, true);

            int visitId = (int)reader["visitId"];
            int staffId = (int)reader["staffId"];

            return new Order(orderNumber, timeCreated, waitingTimeMinutes, status, visitId, staffId);
        }

        public List<Order> GetOrdersByVisitId(int visitId)
        {
            List<Order> orders = new List<Order>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT orderNumber, timeCreated, waitingTimeMinutes, O.[status], O.visitId, O.staffId " +
                    "FROM[Order] AS O " +
                    "JOIN Visit AS V ON V.visitId = O.visitId " +
                    "WHERE V.visitId = @visitId";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@visitId", visitId);

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Order order = ReadOrder(reader);
                    orders.Add(order);
                }

                reader.Close();
            }
            return orders;
        }


        
    }
}