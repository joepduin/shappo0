using Microsoft.Data.SqlClient;
using shappo0.Models;

namespace shappo0.Repositories
{
    public class DbBarRepository : IBarRepository
    {
        private readonly string? _connectionString;
        public DbBarRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("shappo0");
        }

        private Bar ReadBar(SqlDataReader reader)
        {
            int orderNumber = (int)reader["orderNumber"];
            int tableNumber = (int)reader["tableNumber"];
            string status = (string)reader["status"];
            DateTime timeCreated = (DateTime)reader["timeCreated"];
            int waitingTime = (int)reader["waitingTimeMinutes"];

            return new Bar(orderNumber, tableNumber, status, timeCreated, waitingTime);
        }

        public List<Bar> GetAll()
        {
            List<Bar> bars = new List<Bar>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                //double join order - visit - table
                //another join on order - orderline - product where menutype = drink
                // o.status --> shows only the running orders
                //Ordered by timeCreated
                string query = "SELECT  o.orderNumber, o.staffId, t.tableNumber, o.status, o.timeCreated, o.waitingTimeMinutes FROM [ORDER] AS o " +
                    " INNER JOIN VISIT AS v ON o.visitId = v.visitId INNER JOIN [TABLE] AS t ON v.tableNumber = t.tableNumber " +
                    "INNER JOIN ORDER_LINE AS ol ON ol.orderNumber = o.orderNumber INNER JOIN PRODUCT AS p ON ol.productId = p.productId" +
                    " WHERE p.menuType = 'Drink'  AND o.status = 'Running' " +
                    "ORDER BY timeCreated";

                SqlCommand command = new SqlCommand(query, connection);

                command.Connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Bar bar = ReadBar(reader);
                    bars.Add(bar);
                }
                reader.Close();
            }
            return bars;

        }

        public Bar? GetById(int orderNumber)
        {
            Bar bar = null;
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT orderNumber,staffId status, timeCreated FROM ORDER WHERE orderNumber = @orderNumber";

                SqlCommand command = new SqlCommand(query, connection);

                command.Parameters.AddWithValue("@orderNumber", orderNumber);

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    bar = ReadBar(reader);
                }

                reader.Close();
            }
            return bar;
        }
    }
}
