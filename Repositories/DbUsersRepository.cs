using Microsoft.Data.SqlClient;
using shappo0.Models;
using System.Security.Cryptography;
using System.Text;

namespace shappo0.Repositories
{
    public class DbUsersRepository : IUsersRepository
    {
        private readonly string? _connectionString;

        public DbUsersRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("shappo0");
        }

        // Method for getting a user by id, rather than by login credentials
        public User GetUser(int? id)
        {
            User user = null;
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string query = @"SELECT staffId, [name], phoneNumber, sex, [role], [password], isEmployed
                                 FROM Staff
                                 WHERE staffId = @UserId";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserId", id);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            user = ReadUser(reader);
                        }
                    }
                }
                return user;
            }
        }

        //checkt of een gebruiker bestaat, retourneert de gebruiker bij true en fals een lege user
        public User GetUserByCredentials(LoginModel loginModel)
        {
            User user = null;

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                string query = @"SELECT staffId, [name], phoneNumber, sex, [role], [password], isEmployed
                                 FROM Staff
                                 WHERE [password] = @Password AND [name] = @UserName";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Password", loginModel.Password);
                    command.Parameters.AddWithValue("@UserName", loginModel.UserName);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            user = ReadUser(reader);
                        }
                    }
                }
            }
            return user;
        }

        public List<User> GetAllUsers()
        {
            List<User> users = new List<User>();
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
                            User user = ReadUser(reader);
                            users.Add(user);
                        }
                    }
                }
            }
            return users;
        }

        public void AddUser(User user)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = $"INSERT INTO staff ([name], phoneNumber, sex, role, password, isEmployed) " +
                    $"VALUES (@name, @phoneNumber, @sex, @role, @password, @isEmployed); " +
                    $"SELECT SCOPE_IDENTITY();";

                SqlCommand command = new SqlCommand(query, connection);

                command.Parameters.AddWithValue("@name", user.UserName);
                command.Parameters.AddWithValue("@phoneNumber", user.PhoneNumber);
                command.Parameters.AddWithValue("@sex", user.Sex.ToString());
                command.Parameters.AddWithValue("@role", user.Role.ToString());
                command.Parameters.AddWithValue("@password", user.Password);
                command.Parameters.AddWithValue("@isEmployed", user.IsEmployed);

                command.Connection.Open();

                user.UserId= Convert.ToInt32(command.ExecuteScalar());
            }
        }

        public void EditUser(User user)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = $"UPDATE staff " +
                    $"SET [name] = @name, phoneNumber = @phoneNumber, sex = @sex, role = @role, password = @password, isEmployed = @isEmployed " +
                    $"WHERE staffId = @staffId";

                SqlCommand command = new SqlCommand(query, connection);

                command.Parameters.AddWithValue("@staffId", user.UserId);
                command.Parameters.AddWithValue("@name", user.UserName);
                command.Parameters.AddWithValue("@phoneNumber", user.PhoneNumber);
                command.Parameters.AddWithValue("@sex", user.Sex);
                command.Parameters.AddWithValue("@role", user.Role);
                command.Parameters.AddWithValue("@password", user.Password);
                command.Parameters.AddWithValue("@isEmployed", user.IsEmployed);

                command.Connection.Open();

                int nrOfRowsAffected = command.ExecuteNonQuery();
                if (nrOfRowsAffected == 0)
                    throw new Exception("No records updated");
            }
        }

        // Leest een User object uit een SqlDataReader
        private User ReadUser(SqlDataReader reader)
        {
            User user = new User();
            user.UserId = (int)reader["staffId"];
            user.UserName = (string)reader["Name"];
            user.PhoneNumber = reader["phoneNumber"] != DBNull.Value ? (string)reader["phoneNumber"] : null;
            if (reader["sex"] != DBNull.Value)
            {
                string sexString = (string)reader["sex"];
                user.Sex = Enum.Parse<SexType>(sexString);
            }
            else
            {
                user.Sex = null;
            }
            string roleString = (string)reader["role"];
            user.Role = Enum.Parse<RoleType>(roleString);

            user.Password = (string)reader["password"];
            user.IsEmployed = (bool)reader["isEmployed"];

            return user;
        }

    }
}