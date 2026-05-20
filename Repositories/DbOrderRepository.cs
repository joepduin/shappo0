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
                            {
                                order = new Order
                                {
                                    Id = (int)reader["orderNumber"],
                                    TableId = (int)reader["tableNumber"],
                                    OrderItems = new List<OrderLine>()
                                };
                            }

                           
                            decimal vat = (decimal)reader["VATPercentage"];
                            Product product = new Product
                            {
                                ProductId = (int)reader["productId"],
                                Name = (string)reader["name"],
                                Price = (decimal)reader["price"],
                                VATPercentage = vat
                            };

                            
                            OrderLine item = new OrderLine
                            {
                                OrderLineId = (int)reader["orderLineId"],
                                Quantity = (int)reader["quantity"],
                                // attach product to the order line
                                // OrderLine class was extended to include a Product property
                                Product = product
                            };

                            order.OrderItems.Add(item);
                        }
                    }
                }
            }

            return order;
        }
    }
}