using Microsoft.Data.SqlClient;
using shappo0.Enums;
using shappo0.Models;
using shappo0.ViewModels;

namespace shappo0.Repositories
{
    public class DbOrderLineRepository : IOrderLineRepository
    {
        private readonly string? _connectionString;

        public DbOrderLineRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("shappo0");
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

        public List<OrderLine> GetActiveOrderLinesByTableNumber(int tableNumber)
        {
            List<OrderLine> orderLines = new List<OrderLine>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = @"SELECT 
                                    OL.orderLineId, OL.note, OL.quantity, OL.[status] AS OrderLineStatus, 
                                    O.orderNumber, O.timeCreated, O.[Status] AS OrderStatus, 
                                    P.productId, P.[name], P.[price], P.VATPercentage, P.Stock, P.menuType, P.ProductType, P.OnMenu 
                               FROM [Visit] AS V
                               JOIN [Order] AS O ON O.visitId = V.visitId
                               JOIN [Order_Line] AS OL ON OL.orderNumber = O.orderNumber
                               JOIN [Product] AS P ON P.productId = OL.productId
                               WHERE V.TableNumber = @tableNumber AND V.isActive = 1";

                SqlCommand command = new SqlCommand(query, connection);

                command.Parameters.AddWithValue("@tableNumber", tableNumber);

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

        private OrderLine ReadOrderLine(SqlDataReader reader)
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

            OrderViewModel attachedOrder = new OrderViewModel
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
        {
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