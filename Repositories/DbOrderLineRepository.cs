using Microsoft.Data.SqlClient;
using shappo0.Enums;
using shappo0.Models;

namespace shappo0.Repositories
{
    public class DbOrderLineRepository : IOrderLineRepository
    {
        private readonly string? _connectionString;

        public DbOrderLineRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("shappo0");
        }

        public List<OrderLine> GetGroupedOrderLinesByVisitId(int visitId)
        {
            List<OrderLine> orderLines = new List<OrderLine>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = @"SELECT 
                                    P.productId, P.[name], P.[price], P.VATPercentage, P.Stock, P.menuType, P.ProductType, P.OnMenu, 
                                    OT.quantity, OT.orderLineId, OT.orderNumber, OT.timeCreated, OT.OrderStatus, OT.OrderLineStatus, OT.note
                                 FROM (
                                     SELECT 
                                         OL.productId, 
                                         SUM(OL.quantity) AS quantity, MAX(OL.orderLineId) AS orderLineId, MAX(O.orderNumber) AS orderNumber, MAX(O.timeCreated) AS timeCreated, 
                                         MAX(O.[status]) AS OrderStatus, MAX(OL.[status]) AS OrderLineStatus, MAX(OL.note) AS note
                                     FROM [VISIT] AS V
                                     JOIN [ORDER] AS O ON O.visitId = V.visitId
                                     JOIN [ORDER_LINE] AS OL ON OL.orderNumber = O.orderNumber
                                     WHERE V.visitId = @visitId
                                     GROUP BY OL.productId
                                 ) AS OT
                                 JOIN [Product] AS P ON P.productId = OT.productId;";
                //OT = OrderTotal :)

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@visitId", visitId);

                command.Connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    orderLines.Add(ReadOrderLine(reader));
                }
                reader.Close();
            }
            return orderLines;
        }

        public List<OrderLine> GetOrderLinesByOrderNumber(int orderNumber)
        {
            List<OrderLine> orderLines = new List<OrderLine>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {

                string query = @"SELECT     
                                    OL.orderLineId, OL.note, OL.quantity, OL.[status] AS OrderLineStatus, 
                                    O.orderNumber, O.timeCreated, O.[status] AS OrderStatus, 
                                    P.productId, P.[name], P.[price], P.VATPercentage, P.Stock, P.menuType, P.ProductType, P.OnMenu
                               FROM [ORDER_LINE] AS OL
                               JOIN [PRODUCT] AS P ON P.productId = OL.productId
                               JOIN [ORDER] AS O ON O.orderNumber = OL.orderNumber
                               WHERE OL.orderNumber = @orderNumber";

                SqlCommand command = new SqlCommand(query, connection);

                command.Parameters.AddWithValue("@orderNumber", orderNumber);

                command.Connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    OrderLine orderLine = ReadOrderLine(reader);
                    orderLines.Add(orderLine);
                }
                reader.Close();
            }
            return orderLines;
        }

        public List<OrderLine> GetOrderLinesByType(int orderNumber, ProductType type)
        {
            //get orderlines(a list) of a specific order by using @orderNumber (also prevents sql injection)
            //filters on product type --> drink(bar order), !='Drink' --> food(kitchen order)
            List<OrderLine> orderLines = new List<OrderLine>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {

                string query = @"SELECT     
                                    OL.orderLineId, OL.quantity, OL.[status] AS OrderLineStatus, OL.Note,
                                    O.orderNumber, O.timeCreated, O.[status] AS OrderStatus, 
                                    P.productId, P.[name], P.[price], P.VATPercentage, P.Stock, P.menuType, P.ProductType, P.OnMenu
                               FROM [ORDER_LINE] AS OL
                               JOIN [PRODUCT] AS P ON P.productId = OL.productId
                               JOIN [ORDER] AS O ON O.orderNumber = OL.orderNumber
                               WHERE OL.orderNumber = @orderNumber AND ";
                if (type == ProductType.Drink)
                {
                    query += "P.ProductType = 'Drink'";
                }
                else
                {
                    query += "P.ProductType != 'Drink'";
                }

                SqlCommand command = new SqlCommand(query, connection);

                command.Parameters.AddWithValue("@orderNumber", orderNumber);

                command.Connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    OrderLine orderLine = ReadOrderLine(reader);
                    orderLines.Add(orderLine);
                }
                reader.Close();
            }
            return orderLines;
        }

        private static OrderLine ReadOrderLine(SqlDataReader reader)
        {
            Product attachedProduct = new Product
            {
                ProductId = (int)reader["ProductId"],
                Name = (string)reader["name"],
                Price = (decimal)reader["price"],
                VATPercentage = (decimal)reader["vatPercentage"],
                Stock = (int)reader["stock"],
                ProductType = Enum.Parse<ProductType>((string)reader["productType"], true),
                MenuType = Enum.Parse<MenuType>((string)reader["menuType"], true),
                OnMenu = (bool)reader["onMenu"]
            };

            Order attachedOrder = new Order
            {
                OrderNumber = (int)reader["orderNumber"],
                TimeCreated = (DateTime)reader["timeCreated"],
                Status = Enum.Parse<OrderStatus>((string)reader["OrderStatus"], true),
            };

            OrderLine orderLine = new OrderLine
            {
                OrderLineId = (int)reader["orderLineId"],
                Note = reader["note"] as string,
                Quantity = (int)reader["quantity"],
                Status = Enum.Parse<OrderStatus>((string)reader["OrderLineStatus"], true),
                Product = attachedProduct,
                Order = attachedOrder
            };

            return orderLine;
        }

        public void CreateOrderLine(OrderLine orderLine)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = @"INSERT INTO [ORDER_LINE] (productId, orderNumber, note, quantity, status) 
                         VALUES (@productId, @orderNumber, @note, @quantity, @status);";

                SqlCommand command = new SqlCommand(query, connection);

                command.Parameters.AddWithValue("@productId", orderLine.Product.ProductId);
                command.Parameters.AddWithValue("@orderNumber", orderLine.Order.OrderNumber);
                command.Parameters.AddWithValue("@note", string.IsNullOrEmpty(orderLine.Note) ? DBNull.Value : orderLine.Note);
                command.Parameters.AddWithValue("@quantity", orderLine.Quantity);
                command.Parameters.AddWithValue("@status", orderLine.Status.ToString());

                command.Connection.Open();

                command.ExecuteNonQuery();
            }
        }

        public void CreateOrderLines(List<OrderLine> orderLines)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = @"INSERT INTO [ORDER_LINE] (productId, orderNumber, note, quantity, status) 
                         VALUES (@productId, @orderNumber, @note, @quantity, @status);";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Connection.Open();

                    foreach (OrderLine orderLine in orderLines)
                    {
                        command.Parameters.Clear();

                        command.Parameters.AddWithValue("@productId", orderLine.Product.ProductId);
                        command.Parameters.AddWithValue("@orderNumber", orderLine.Order.OrderNumber);

                        command.Parameters.AddWithValue("@note", string.IsNullOrEmpty(orderLine.Note) ? DBNull.Value : orderLine.Note);
                        command.Parameters.AddWithValue("@quantity", orderLine.Quantity);
                        command.Parameters.AddWithValue("@status", orderLine.Status.ToString());

                        command.ExecuteNonQuery();
                    }
                }
            }
        }

        public void UpdateOrderLineStatus(int orderLineId, OrderStatus status)
        { //Update specific orderline status (ordered - being prepared - ready to be served)
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

        public void UpdateCourseStatus(int orderNumber, ProductType productType, OrderStatus status)
        { 
            //kitchen exclusive method
            //Change the status of a specific course within a food order (ordered - being prepared - ready to be served)
            
            using SqlConnection connection = new SqlConnection(_connectionString);

            string query =
            "UPDATE ol " +
            "SET ol.status = @status " +
            "FROM ORDER_LINE AS ol " +
            "INNER JOIN PRODUCT AS p ON p.productId = ol.productId " +
            "WHERE orderNumber = @orderNumber AND p.productType = @productType";

            SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@status", status.ToString());
            command.Parameters.AddWithValue("@orderNumber", orderNumber);
            command.Parameters.AddWithValue("@productType", productType.ToString());

            connection.Open();
            command.ExecuteNonQuery();
        }

        public void UpdateOrderStatus(int orderNumber, OrderStatus status)
        { // updates the whole status order from running to served
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
    }
}