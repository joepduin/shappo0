using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using shappo0.Models;
using shappo0.Repositories.Chapeau.Repositories;

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
                                    OrderItems = new List<OrderItem>()
                                };
                            }

                           
                            decimal vat = (decimal)reader["VATPercentage"];
                            MenuItem menuItem = new MenuItem
                            {
                                Id = (int)reader["productId"],
                                Name = (string)reader["name"],
                                Price = (double)(decimal)reader["price"],
                                IsAlcoholic = (vat == 21)
                            };

                           
                            OrderItem item = new OrderItem
                            {
                                Id = (int)reader["orderLineId"],
                                Count = (int)reader["quantity"],
                                MenuItem = menuItem
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