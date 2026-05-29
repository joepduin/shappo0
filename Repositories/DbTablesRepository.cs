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

                string query = @"SELECT tableNumber, [status]
                                 FROM [TABLE]";

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
            table.TableNumber = reader.GetInt32(0);
            table.Status = reader.GetBoolean(1);
            return table;
        }

        
    }
}