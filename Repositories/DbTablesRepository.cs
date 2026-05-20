using shappo0.Models;
using Microsoft.Data.SqlClient;

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

                string query = @"SELECT TA.tableNumber, TA.[status] AS 'tableStatus',

                                (SELECT OD.[status]
                                FROM ORDER_LINE AS OL
                                JOIN [PRODUCT] AS PR ON OL.productId = PR.productId
                                JOIN [ORDER] AS OD ON OL.orderNumber = OD.orderNumber
                                JOIN VISIT AS VI ON OD.visitId = VI.visitId
                                WHERE PR.menuType = 'Drink' AND VI.tableNumber = TA.tableNumber) AS 'foodStatus',

                                (SELECT OD.[status]
                                FROM ORDER_LINE AS OL
                                JOIN [PRODUCT] AS PR ON OL.productId = PR.productId
                                JOIN [ORDER] AS OD ON OL.orderNumber = OD.orderNumber
                                JOIN VISIT AS VI ON OD.visitId = VI.visitId
                                WHERE PR.menuType != 'Drink' AND VI.tableNumber = TA.tableNumber) AS 'drinkStatus' 

                                FROM [TABLE] AS TA
                                ORDER BY tableNumber;";

                using (SqlCommand command = new SqlCommand(query, connection))
                {

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Table table = ReadTable(reader);
                            tables.Add(table);
                        }
                    }
                }
            }
            return tables;
        }

        //geeft per tafel het tafel opject
        private Table ReadTable(SqlDataReader reader)
        {
            Table table = new Table();
            table.TableNumber = (int)reader["tableNumber"];

            if ((bool)reader["tableStatus"]) table.Status = TableStatusType.Occupied;
            else table.Status = TableStatusType.Available;

            if (reader["foodStatus"] != DBNull.Value) table.FoodStatus = Enum.Parse<OrderStatus>((string)reader["foodStatus"]);
            else table.FoodStatus = OrderStatus.None;

            if (reader["drinkStatus"] != DBNull.Value) table.DrinkStatus = Enum.Parse<OrderStatus>((string)reader["drinkStatus"]);
            else table.DrinkStatus = OrderStatus.None;

            return table;
        }
    }
}