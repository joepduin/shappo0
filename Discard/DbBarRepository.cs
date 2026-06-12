using Microsoft.Data.SqlClient;
using shappo0.Models;
using shappo0.Enums;
using shappo0.ViewModels;

namespace shappo0.Discard
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
            OrderStatus status = Enum.Parse<OrderStatus>((string)reader["status"], true);
            DateTime timeCreated = (DateTime)reader["timeCreated"];

            return new Bar(orderNumber, tableNumber, status, timeCreated);
        }

        public List<Bar> GetAll()
        {
            List<Bar> bars = new List<Bar>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            { //double join order - visit - table
              //another join on order - orderline - product where ProductType drink
              //Uses DISTINCT so not multiple orders will be seen
              // o.status --> shows only the running orders
              //Ordered by timeCreated
                string query = "SELECT DISTINCT o.orderNumber, o.staffId, t.tableNumber, o.status, o.timeCreated FROM [ORDER] AS o " +
                    " INNER JOIN VISIT AS v ON o.visitId = v.visitId INNER JOIN [TABLE] AS t ON v.tableNumber = t.tableNumber " +
                    "INNER JOIN ORDER_LINE AS ol ON ol.orderNumber = o.orderNumber INNER JOIN PRODUCT AS p ON ol.productId = p.productId " +
                    "WHERE o.status != 'Served' and p.ProductType = 'Drink' " +
                    "ORDER BY timeCreated"
;

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
                string query = "SELECT orderNumber,staffId, status, timeCreated FROM [ORDER] WHERE orderNumber = @orderNumber";

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

        public List<OrderLine> GetOrderLines(int orderNumber)
        {
            //Gets all orderlines from a specific order
            List<OrderLine> orderLines = new List<OrderLine>();

            
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query =
                    "SELECT ol.orderLineId, ol.productId, ol.orderNumber, " +
                    "ol.note, ol.quantity, ol.status, " +
                    "p.name,  p.productType " +
                    "FROM [ORDER_LINE] AS ol " +
                    "INNER JOIN [ORDER] AS o ON o.orderNumber = ol.orderNumber " +
                    "INNER JOIN [PRODUCT] AS p ON p.productId = ol.productId " +
                    "WHERE ol.orderNumber = @orderNumber " +
                    "AND p.productType = 'Drink' ";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@orderNumber", orderNumber);

                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())//
                {
                    Product product = new Product
                    {
                        ProductId = (int)reader["ProductId"],
                        Name = (string)reader["name"]
                    };

                    OrderViewModel order = new OrderViewModel
                    {
                        OrderNumber = (int)reader["orderNumber"]
                    };

                    OrderLine orderLine = new OrderLine
                    {
                        OrderLineId = (int)reader["orderLineId"],
                        Note = reader["note"] as string,
                        Quantity = (int)reader["quantity"],
                        Status = Enum.Parse<OrderStatus>((string)reader["Status"], true),
                        Product = product,
                        Order = order
                    };
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
        { //updates each order line from ordered --> being prepared --> ready to serve
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

        public List<Bar> FinishedOrders()
        {
            List<Bar> bars = new List<Bar>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            { //double join order - visit - table
              //another join on order - orderline - product where productType Drink
              // o.status --> shows only the finished orders
              //Ordered by timeCreated
                string query = "SELECT DISTINCT o.orderNumber, o.staffId, t.tableNumber, o.status, o.timeCreated FROM [ORDER] AS o " +
                    " INNER JOIN VISIT AS v ON o.visitId = v.visitId INNER JOIN [TABLE] AS t ON v.tableNumber = t.tableNumber " +
                    "INNER JOIN ORDER_LINE AS ol ON ol.orderNumber = o.orderNumber INNER JOIN PRODUCT AS p ON ol.productId = p.productId" +
                    " WHERE p.ProductType = 'Drink' AND o.status = 'Served' " +
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
    }
}

