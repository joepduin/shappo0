using Microsoft.Data.SqlClient;
using shappo0.Models;

namespace shappo0.Repositories
{
    public class DbKitchenRepository : IKitchenRepository
    {
        private readonly string? _connectionString;
        public DbKitchenRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("shappo0");
        }

        public static Kitchen ReadKitchen(SqlDataReader reader)
        {
            int id = (int)reader["orderNumber"];
            List<OrderLine> orderLines = (List<OrderLine>)reader["OrderLine"];
            string status = (string)reader["status"];
            DateTime timeCreated = (DateTime)reader["timeCreated"];
            DateTime waitingTime = (DateTime)reader["waitingTime"];

            return new Kitchen(id,orderLines, status, timeCreated, waitingTime);
        }

        public List<Kitchen> GetAll()
        {
            List<Kitchen> kitchens = new List<Kitchen>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            { //double join order - visit - table
              //another join on order - orderline - product where menutype lunch/dinner
                string query = "SELECT orderNumber,staffId, tableNumber, status, timeCreated, waitingTime FROM ORDER AS o " +
                    " INNER JOIN VISIT AS v ON o.visitId = v.visitId INNER JOIN TABLE AS t ON v.tableNumber ON t.tableNumber " +
                    "INNER JOIN ORDER_LINE AS ol ON ol.orderNumber = o.orderNumber INNER JOIN PRODUCT AS p ON ol.productId = p.productId" +
                    "WHERE menuType != 'Drink' AND status = 'running' " +
                    "ORDER BY timeCreated";

                SqlCommand command = new SqlCommand(query, connection);

                command.Connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Kitchen kitchen = ReadKitchen(reader);
                    kitchens.Add(kitchen);
                }
                reader.Close();
            }
            return kitchens;

        }

        public Kitchen? GetById(int orderNumber)
        {
            Kitchen kitchen = null;
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT orderNumber,staffId, status, timeCreated FROM ORDER WHERE orderNumber = @orderNumber";

                SqlCommand command = new SqlCommand(query, connection);

                command.Parameters.AddWithValue("@orderNumber", orderNumber);

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    kitchen = ReadKitchen(reader);
                }

                reader.Close();
            }
            return kitchen;
        }
    }
}
