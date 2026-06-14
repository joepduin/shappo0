using Microsoft.Data.SqlClient;
using shappo0.Enums;
using shappo0.Models;

namespace shappo0.Repositories
{
    public class DbStaffRepository : IStaffRepository
    {
        private readonly string? _connectionString;

        public DbStaffRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("shappo0");
        }

        // Joep
        //checkt of een stafflid bestaat, retourneert de staff bij true en false een lege staff
        public Staff? GetStaffByCredentials(LoginModel loginModel)
        {
            Staff? staff = null;
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = @"SELECT staffId, [name], phoneNumber, sex, [role], [password], isEmployed
                                 FROM Staff
                                 WHERE [password] = @Password AND [name] = @Name";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Password", loginModel.Password);
                    command.Parameters.AddWithValue("@Name", loginModel.Name);

                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            staff = ReadStaff(reader);
                        }
                    }
                }
            }
            return staff;
        }

        // Ylvi
        // Method for getting a staff member by id, rather than by login credentials
        public Staff? GetStaff(int? id)
        {
            Staff? staff = null;
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string query = @"SELECT staffId, [name], phoneNumber, sex, [role], [password], isEmployed
                                 FROM Staff
                                 WHERE staffId = @StaffId";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@StaffId", id);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            staff = ReadStaff(reader);
                        }
                    }
                }
                return staff;
            }
        }

        public List<Staff> GetAllStaff()
        {
            List<Staff> staffList = new List<Staff>();
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string query = @"SELECT staffId, [name], phoneNumber, sex, [role], [password], isEmployed " +
                                 "FROM Staff";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Staff staff = ReadStaff(reader);
                            staffList.Add(staff);
                        }
                    }
                }
            }
            return staffList;
        }

        public void AddStaff(Staff staff)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = $"INSERT INTO staff ([name], phoneNumber, sex, role, password, isEmployed) " +
                    $"VALUES (@name, @phoneNumber, @sex, @role, @password, @isEmployed); " +
                    $"SELECT SCOPE_IDENTITY();";

                SqlCommand command = new SqlCommand(query, connection);

                command.Parameters.AddWithValue("@name", staff.Name);
                command.Parameters.AddWithValue("@phoneNumber", staff.PhoneNumber);
                command.Parameters.AddWithValue("@sex", staff.Sex.ToString());
                command.Parameters.AddWithValue("@role", staff.Role.ToString());
                command.Parameters.AddWithValue("@password", staff.Password);
                command.Parameters.AddWithValue("@isEmployed", staff.IsEmployed);

                command.Connection.Open();

                staff.StaffId = Convert.ToInt32(command.ExecuteScalar());
            }
        }

        public void EditStaff(Staff staff)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = $"UPDATE staff " +
                    $"SET [name] = @name, phoneNumber = @phoneNumber, sex = @sex, role = @role, password = @password, isEmployed = @isEmployed " +
                    $"WHERE staffId = @staffId";

                SqlCommand command = new SqlCommand(query, connection);

                command.Parameters.AddWithValue("@staffId", staff.StaffId);
                command.Parameters.AddWithValue("@name", staff.Name);
                command.Parameters.AddWithValue("@phoneNumber", staff.PhoneNumber);
                command.Parameters.AddWithValue("@sex", staff.Sex.ToString());
                command.Parameters.AddWithValue("@role", staff.Role.ToString());
                command.Parameters.AddWithValue("@password", staff.Password);
                command.Parameters.AddWithValue("@isEmployed", staff.IsEmployed);

                command.Connection.Open();

                int nrOfRowsAffected = command.ExecuteNonQuery();
                if (nrOfRowsAffected == 0)
                    throw new Exception("No records updated");
            }
        }

        // Joep
        // Leest een Staff object uit een SqlDataReader
        private Staff ReadStaff(SqlDataReader reader)
        {
            return new Staff
            {
                StaffId = (int)reader["staffId"],
                Name = (string)reader["name"],
                PhoneNumber = (string)reader["phoneNumber"],
                Password = (string)reader["password"],
                IsEmployed = (bool)reader["isEmployed"],
                Sex = Enum.Parse<SexType>((string)reader["sex"], true),
                Role = Enum.Parse<RoleType>((string)reader["role"], true)
            };
        }
    }
}