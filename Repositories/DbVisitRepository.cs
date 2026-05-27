using Microsoft.Data.SqlClient;
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
            int visitId = (int)reader["visitId"];
            int tableNumber = (int)reader["tableNumber"];
            int staffId = (int)reader["staffId"];
            bool isActive = (bool)reader["isActive"];

            return new Visit(visitId, tableNumber, staffId, isActive);
        }

        public Visit? GetById(int id)
        {
            Visit visit = null;

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT visitId, tableNumber, staffId, isActive " +
                    "FROM [Visit] " +
                    "WHERE visitId = @visitId";

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

        public Visit GetActiveVisitForTable(int tableNumber)
        {
            Visit visit = null;

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT visitId, tableNumber, staffId, isActive " +
                            "FROM [Visit] " +
                            "WHERE tableNumber = @tableNumber AND isActive = 1";

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
                string query = $"INSERT INTO Visit(tableNumber, staffId, isActive) " +
                                $"VALUES(@tableNumber, @staffId, @isActive); " +
                                "SELECT SCOPE_IDENTITY();";

                SqlCommand command = new SqlCommand(query, connection);

                command.Parameters.AddWithValue("@tableNumber", visit.TableNumber);
                command.Parameters.AddWithValue("@staffId", visit.StaffId);
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
                
                string query = "UPDATE [VISIT] SET isActive = 0 WHERE visitId = @visitId";

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

        //deze methode word nu nog niet gebruikt, want ik vond een andere handiger, maar ik laat hem er voor nu in. misschien heeft iemand anders het nodig.
        public List<Visit> GetActiveVisits()
        {
            List<Visit> visits = new List<Visit>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = @"SELECT visitId, tableNumber, staffId, isActive
                                 FROM [Visit] 
                                 WHERE isActive = 1";

                SqlCommand command = new SqlCommand(query, connection);

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Visit visit = ReadVisit(reader);
                    visits.Add(visit);
                }

                reader.Close();
            }
            return visits;
        }
    }
}
