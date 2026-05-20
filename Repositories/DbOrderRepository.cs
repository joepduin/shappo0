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

        public Order GetOrderForTable(int tableId)
        {
            Order order = null;

            string query = @"
                SELECT o.orderNumber, v.tableNumber, 
                       ol.orderLineId, ol.quantity, 
                       p.productId, p.name, p.price, p.VATPercentage 
                FROM [ORDER] o
                JOIN VISIT v ON o.visitId = v.visitId
                JOIN ORDER_LINE ol ON o.orderNumber = ol.orderNumber
                JOIN [PRODUCT] p ON ol.productId = p.productId
                WHERE v.tableNumber = @tableId";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@tableId", tableId);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (order == null)
                                order = ReadOrderHeader(reader);

                            Product product = ReadProductFromReader(reader);
                            OrderLine item = ReadOrderLineFromReader(reader, product);

                            order.OrderItems.Add(item);
                        }
                    }
                }
            }

            return order;
        }

        // Lees order header (ordernummer en tafelnummer) uit de reader
        private Order ReadOrderHeader(SqlDataReader reader)
        {
            return new Order
            {
                Id = (int)reader["orderNumber"],
                TableId = (int)reader["tableNumber"],
                OrderItems = new List<OrderLine>()
            };
        }

        // Maak een Product object van de huidige reader-rij
        private Product ReadProductFromReader(SqlDataReader reader)
        {
            decimal vat = (decimal)reader["VATPercentage"];
            return new Product
            {
                ProductId = (int)reader["productId"],
                Name = (string)reader["name"],
                Price = (decimal)reader["price"],
                VATPercentage = vat
            };
        }

        // Maak een OrderLine object van de huidige reader-rij en koppel het product
        private OrderLine ReadOrderLineFromReader(SqlDataReader reader, Product product)
        {
            return new OrderLine
            {
                OrderLineId = (int)reader["orderLineId"],
                Quantity = (int)reader["quantity"],
                Product = product
            };
        }
    }
}