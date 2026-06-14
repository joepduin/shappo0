using Microsoft.Data.SqlClient;
using shappo0.Enums;
using shappo0.Models;

namespace shappo0.Repositories
{
    public class DbVisitRepository : IVisitRepository
    {
        private readonly string? _connectionString;

        public DbVisitRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("shappo0");
        }

        private Visit ReadVisit(SqlDataReader reader)
        {

            Table attachedTable = new Table
            {
                TableNumber = (int)reader["tableNumber"],
                Status = (bool)reader["TableStatus"]
            };

            Staff attachedStaff = new Staff
            {
                StaffId = (int)reader["staffId"],
                Name = (string)reader["name"],
                Role = Enum.Parse<RoleType>((string)reader["role"], true)
            };

            int visitId = (int)reader["visitId"];
            bool isActive = (bool)reader["isActive"];

            return new Visit(visitId, attachedTable, attachedStaff, isActive);
        }

        public Visit? GetById(int id)
        {
            Visit? visit = null;

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = @"
                    SELECT V.visitId, V.tableNumber, V.isActive, 
                           S.staffId, S.name, S.role,
                           T.status AS TableStatus
                    FROM [Visit] AS V
                    JOIN [Staff] AS S ON V.staffId = S.staffId
                    JOIN [Table] AS T ON V.tableNumber = T.tableNumber
                    WHERE V.visitId = @visitId";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@visitId", id);

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    visit = ReadVisit(reader);
                }

                reader.Close();
            }
            return visit;
        }

        public Visit? GetActiveVisitForTable(int tableNumber)
        {
            Visit? visit = null;

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = @"
                    SELECT V.visitId, V.tableNumber, V.isActive, 
                           S.staffId, S.name, S.role,
                           T.status AS TableStatus
                    FROM [Visit] AS V
                    JOIN [Staff] AS S ON V.staffId = S.staffId
                    JOIN [Table] AS T ON V.tableNumber = T.tableNumber
                    WHERE V.tableNumber = @tableNumber AND V.isActive = 1";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@tableNumber", tableNumber);

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    visit = ReadVisit(reader);
                }

                reader.Close();
            }
            return visit;
        }

        public int StartNewVisit(Visit visit)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = @"
                    INSERT INTO [Visit] (tableNumber, staffId, isActive) 
                    VALUES (@tableNumber, @staffId, @isActive); 
                    SELECT SCOPE_IDENTITY();";

                SqlCommand command = new SqlCommand(query, connection);

                command.Parameters.AddWithValue("@tableNumber", visit.Table.TableNumber);
                command.Parameters.AddWithValue("@staffId", visit.Staff.StaffId);
                command.Parameters.AddWithValue("@isActive", visit.IsActive);

                command.Connection.Open();

                visit.VisitId = Convert.ToInt32(command.ExecuteScalar());

                return visit.VisitId;
            }
        }

        public void CloseVisit(int visitId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = @"
                    UPDATE [Visit] 
                    SET isActive = 0 
                    WHERE visitId = @visitId";

                SqlCommand command = new SqlCommand(query, connection);

                command.Parameters.AddWithValue("@visitId", visitId);
                command.Connection.Open();

                int nrOfRowsAffected = command.ExecuteNonQuery();
                if (nrOfRowsAffected == 0)
                {
                    throw new Exception("No records updated!");
                }
            }
        }
    }
}