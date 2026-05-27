using Microsoft.Data.SqlClient;
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

        public List<OrderLine> GetOrderLinesByOrderNumber(int orderNumber)
        {
            List<OrderLine> orderLines = new List<OrderLine>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT OL.orderLineId, OL.productId, OL.orderNumber, OL.note, OL.quantity, OL.status, " +
                    "P.productId, P.[name], P.[price], P.VATPercentage, P.Stock, P.menuType, P.ProductType, P.OnMenu " +
                    "FROM [ORDER_LINE] AS OL " + 
                    "JOIN [PRODUCT] AS P ON P.productId = OL.productId " +
                    "WHERE orderNumber = @orderNumber";

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

        private OrderLine ReadOrderLine(SqlDataReader reader)
        {
            OrderLine line = new OrderLine();
            line.OrderLineId = (int)reader["orderLineId"];
            line.ProductId = (int)reader["productId"];
            line.OrderNumber = (int)reader["orderNumber"];
            //Saves the note as a C# string, otherwise it crashes if it finds a NULL value
            line.Note = reader["note"] as string;
            line.Quantity = (int)reader["quantity"];
  
            string statusString = (string)reader["status"];
            line.Status = Enum.Parse<OrderStatus>(statusString, true);

            Product attachedProduct = DbMenuRepository.ReadProduct(reader);
            line.Product = attachedProduct;
            return line;
        }

        public void CreateOrderLine(OrderLine orderLine)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = @"INSERT INTO [ORDER_LINE] (productId, orderNumber, note, quantity, status) 
                         VALUES (@productId, @orderNumber, @note, @quantity, @status);";

                SqlCommand command = new SqlCommand(query, connection);

                command.Parameters.AddWithValue("@productId", orderLine.Product.ProductId);
                command.Parameters.AddWithValue("@orderNumber", orderLine.OrderNumber);
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
                        //Clear the parameters from previous loop
                        command.Parameters.Clear();

                        command.Parameters.AddWithValue("@productId", orderLine.Product.ProductId);
                        command.Parameters.AddWithValue("@orderNumber", orderLine.OrderNumber);
                        command.Parameters.AddWithValue("@note", string.IsNullOrEmpty(orderLine.Note) ? DBNull.Value : orderLine.Note);
                        command.Parameters.AddWithValue("@quantity", orderLine.Quantity);
                        command.Parameters.AddWithValue("@status", orderLine.Status.ToString());

                        command.ExecuteNonQuery();
                    }
                    
                }
            }
        }


    }
}
