using Microsoft.Data.SqlClient;
using shappo0.Enums;
using shappo0.Models;

namespace shappo0.Repositories
{
    public class DbTablesRepository : ITablesRepository
    {
        private readonly string? _connectionString;

        public DbTablesRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("shappo0");
        }

        //geeft een list van alle tafels
        public List<Table> GetAllTables()
        {
            List<Table> tables = new List<Table>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                string query = @"SELECT TA.tableNumber, TA.status AS tableStatus, OD.orderNumber, OD.status AS orderStatus, OL.status AS orderLineStatus, OL.quantity, OL.note, PR.name, PR.productType
                                 FROM [TABLE] AS TA
                                 LEFT JOIN [VISIT] AS VI ON VI.tableNumber = TA.tableNumber AND VI.isActive = 1
                                 LEFT JOIN [ORDER] AS OD ON OD.visitId = VI.visitId
                                 LEFT JOIN [ORDER_LINE] AS OL ON OL.orderNumber = OD.orderNumber
                                 LEFT JOIN [PRODUCT] AS PR ON PR.productId = OL.productId
                                 ORDER BY TA.tableNumber;";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ReadTable(reader, tables);
                        }
                    }
                }
            }
            return tables;
        }

        public Table GetTableByNumber(int tableNumber)
        {
            List<Table> tables = new List<Table>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                string query = @"SELECT TA.tableNumber, TA.status AS tableStatus, OD.orderNumber, OD.status AS orderStatus, OL.status AS orderLineStatus, OL.quantity, OL.note, PR.name, PR.productType
                                 FROM [TABLE] AS TA
                                 LEFT JOIN [VISIT] AS VI ON VI.tableNumber = TA.tableNumber AND VI.isActive = 1
                                 LEFT JOIN [ORDER] AS OD ON OD.visitId = VI.visitId
                                 LEFT JOIN [ORDER_LINE] AS OL ON OL.orderNumber = OD.orderNumber
                                 LEFT JOIN [PRODUCT] AS PR ON PR.productId = OL.productId
                                 WHERE TA.tableNumber = @tableNumber
                                 ORDER BY TA.tableNumber;";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@tableNumber", tableNumber);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ReadTable(reader, tables);
                        }
                    }
                }
            }
            return tables.FirstOrDefault(); //de readtable methode werkt alleen met list, aangezien er maar 1 table is retourn neer alleen de eerste van de list.
        }


        private void ReadTable(SqlDataReader reader, List<Table> tables)
        {
            // checkt of er al een tafel bestaat in de list, anders word het aangemaakt
            int tableNumber = (int)reader["tableNumber"];
            Table? currentTable = tables.FirstOrDefault(t => t.TableNumber == tableNumber);

            if (currentTable == null)
            {
                currentTable = new Table
                {
                    TableNumber = tableNumber,
                    Status = (bool)reader["tableStatus"],
                    Orders = new List<Order>()
                };
                tables.Add(currentTable);
            }

            // voegd de order toe aan de list als hij nog niet bestaat
            if (reader["orderNumber"] != DBNull.Value)
            {
                int orderNumber = (int)reader["orderNumber"];
                Order? currentOrder = currentTable.Orders.FirstOrDefault(o => o.OrderNumber == orderNumber);

                if (currentOrder == null)
                {
                    currentOrder = new Order
                    {
                        OrderNumber = orderNumber,
                        Status = Enum.Parse<OrderStatus>((string)reader["orderStatus"]),
                        OrderLines = new List<OrderLine>()
                    };
                    currentTable.Orders.Add(currentOrder);
                }

                // voegd de ordeline en product toe aan de list
                if (reader["name"] != DBNull.Value)
                {
                    Product product = new Product
                    {
                        Name = (string)reader["name"],
                        ProductType = Enum.Parse<ProductType>((string)reader["productType"])
                    };

                    OrderLine orderLine = new OrderLine
                    {
                        Product = product,
                        Order = currentOrder,
                        Note = reader["note"] as string,
                        Quantity = (int)reader["quantity"],
                        Status = Enum.Parse<OrderStatus>((string)reader["orderLineStatus"])
                    };

                    currentOrder.OrderLines.Add(orderLine);
                }
            }
        }
    }
}