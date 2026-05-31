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

        private Kitchen ReadKitchen(SqlDataReader reader)
        {
            int orderNumber = (int)reader["orderNumber"];
            int tableNumber = (int)reader["tableNumber"];
            string status = (string)reader["status"];
            DateTime timeCreated = (DateTime)reader["timeCreated"];
            int waitingTime = (int)reader["waitingTimeMinutes"];

            return new Kitchen(orderNumber, tableNumber, status, timeCreated, waitingTime);
        }

        public List<Kitchen> GetAll()
        {
            List<Kitchen> kitchens = new List<Kitchen>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            { //double join order - visit - table
              //another join on order - orderline - product where productType != Drink
              //Uses DISTINCT so not multiple orders will be seen
              // o.status --> shows only the running orders
              //Ordered by timeCreated
                string query = "SELECT DISTINCT o.orderNumber, o.staffId, t.tableNumber, o.status, o.timeCreated, o.waitingTimeMinutes FROM [ORDER] AS o " +
                    " INNER JOIN VISIT AS v ON o.visitId = v.visitId INNER JOIN [TABLE] AS t ON v.tableNumber = t.tableNumber " +
                    "INNER JOIN ORDER_LINE AS ol ON ol.orderNumber = o.orderNumber INNER JOIN PRODUCT AS p ON ol.productId = p.productId" +
                    " WHERE p.ProductType != 'Drink' AND o.status = 'Running' " +
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
                string query = "SELECT orderNumber,staffId, status, timeCreated FROM [ORDER] WHERE orderNumber = @orderNumber";

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

        public List<OrderLine> GetOrderLines(int orderNumber)
        {  //get the order lines of a specific order
            List<OrderLine> orderLines = new List<OrderLine>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query =
                    "SELECT ol.orderLineId, ol.productId, ol.orderNumber, " +
                    "ol.note, ol.quantity, ol.status, " +
                    "p.name, p.menuType, p.productType " +
                    "FROM [ORDER_LINE] AS ol " +
                    "INNER JOIN [ORDER] AS o ON o.orderNumber = ol.orderNumber " +
                    "INNER JOIN [PRODUCT] AS p ON p.productId = ol.productId " +
                    "WHERE ol.orderNumber = @orderNumber " +
                    "AND p.productType != 'Drink' " +
                    "ORDER BY p.productType";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@orderNumber", orderNumber);

                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    OrderLine orderLine = new OrderLine();

                    orderLine.OrderLineId = int.Parse(reader["orderLineId"].ToString());
                    orderLine.ProductId = int.Parse(reader["productId"].ToString());
                    orderLine.OrderNumber = int.Parse(reader["orderNumber"].ToString());
                    orderLine.Note = reader["note"].ToString();
                    orderLine.Quantity = int.Parse(reader["quantity"].ToString());

                   
                    if (Enum.TryParse<OrderStatus>(reader["status"].ToString(), out var status))
                    {
                        orderLine.Status = status;
                    }
                    
                    orderLines.Add(orderLine);

                    
                }
                reader.Close();
            }

            return orderLines;
        }

        public void UpdateOrderStatus(int orderNumber, OrderStatus status)
        { // updates the whole status order from running to finished
            using SqlConnection connection = new SqlConnection(_connectionString);

            string query =
                "UPDATE [ORDER] " +
                "SET status = @status " +
                "WHERE orderNumber = @orderNumber";

            SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@status", status.ToString());
            command.Parameters.AddWithValue("@orderNumber", orderNumber);

            connection.Open();
            command.ExecuteNonQuery();
        }

        public void UpdateOrderLineStatus(int orderLineId, OrderStatus status)
        {//updates each order line from ordered --> being prepared --> ready to serve
            using SqlConnection connection = new SqlConnection(_connectionString);

            string query =
                "UPDATE [ORDER_LINE] " +
                "SET status = @status " +
                "WHERE orderLineId = @orderLineId";

            SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@status", status.ToString());
            command.Parameters.AddWithValue("@orderLineId", orderLineId);

            connection.Open();
            command.ExecuteNonQuery();
        }

        public List<Kitchen> FinishedOrders()
        {
            List<Kitchen> kitchens = new List<Kitchen>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            { //double join order - visit - table
              //another join on order - orderline - product where productType!= Drink
              // o.status --> shows only the finished orders
              //Ordered by timeCreated
                string query = "SELECT DISTINCT o.orderNumber, o.staffId, t.tableNumber, o.status, o.timeCreated, o.waitingTimeMinutes FROM [ORDER] AS o " +
                    " INNER JOIN VISIT AS v ON o.visitId = v.visitId INNER JOIN [TABLE] AS t ON v.tableNumber = t.tableNumber " +
                    "INNER JOIN ORDER_LINE AS ol ON ol.orderNumber = o.orderNumber INNER JOIN PRODUCT AS p ON ol.productId = p.productId" +
                    " WHERE p.ProductType != 'Drink' AND o.status = 'Finished' " +
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
    }
}

