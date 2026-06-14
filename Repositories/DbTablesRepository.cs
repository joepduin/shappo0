using Microsoft.Data.SqlClient;
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
                string query = @"SELECT tableNumber, status
                                 FROM [TABLE]
                                 ORDER BY tableNumber;";

                connection.Open();
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

        //Geeft 1 tafel object
        public Table? GetTableByNumber(int tableNumber)
        {
            Table? table = null;
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = @"SELECT tableNumber, status
                                 FROM [TABLE]
                                 WHERE tableNumber = @tableNumber
                                 ORDER BY tableNumber;";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@tableNumber", tableNumber);

                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            table = ReadTable(reader);
                        }
                    }
                }
            }
            return table;
        }

        //verander de tafel status
        public void ChangeTableStatus(Table table)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = @"UPDATE [TABLE]
                                 SET status = @status
                                 WHERE tableNumber = @tableNumber";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@tableNumber", table.TableNumber);
                    command.Parameters.AddWithValue("@status", table.Status);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        //Leest het tafel object
        private Table ReadTable(SqlDataReader reader)
        {
            return new Table
            {
                TableNumber = (int)reader["tableNumber"],
                Status = (bool)reader["status"]
            };
        }
    }
}