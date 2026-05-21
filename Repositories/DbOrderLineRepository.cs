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
            line.Note = (string)reader["note"];
            line.Quantity = (int)reader["quantity"];
  
            string statusString = (string)reader["status"];
            line.Status = Enum.Parse<OrderStatus>(statusString, true);

            Product attachedProduct = new Product();
            attachedProduct.ProductId = (int)reader["productId"];
            attachedProduct.Name = (string)reader["Name"];
            attachedProduct.Price = (decimal)reader["Price"];
            attachedProduct.VATPercentage = (decimal)reader["VATPercentage"];
            attachedProduct.Stock = (int)reader["Stock"];

            string productTypeString = (string)reader["productType"];
            string menuTypeString = (string)reader["menuType"];

            attachedProduct.ProductType = Enum.Parse<ProductType>(productTypeString, true);
            attachedProduct.MenuType = Enum.Parse<MenuType>(menuTypeString, true);
            attachedProduct.OnMenu = (bool)reader["OnMenu"];


            line.Product = attachedProduct;
            return line;
        }
    }
}
