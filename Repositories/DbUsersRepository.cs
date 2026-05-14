using System.Data.SqlClient;
using shappo0.Models;
using System.Security.Cryptography;
using System.Text;

namespace shappo0.Repositories
{
    public class DbUsersRepository : IUsersRepository
    {
        private readonly string _connectionString;

        public DbUsersRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("shappo0");
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
                    command.Parameters.AddWithValue("@Password", HashPassword(loginModel.Password));
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

        // Leest een User object uit een SqlDataReader
        private User ReadUser(SqlDataReader reader)
        {
            User user = new User();
            user.UserId = reader.GetInt32(0);
            user.UserName = reader.GetString(1);
            user.PhoneNumber = reader.IsDBNull(2) ? null : reader.GetString(2);
            user.Sex = reader.IsDBNull(3) ? null : Enum.Parse<Sexe>(reader.GetString(3));
            user.Role = Enum.Parse<Roles>(reader.GetString(4));
            user.Password = reader.GetString(5);
            user.IsEmployed = reader.GetBoolean(6);
            return user;
        }

        //maakt een hash van het wachtwoord
        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                return Convert.ToBase64String(sha256.ComputeHash(Encoding.UTF8.GetBytes(password)));
            }
        }
    }
}