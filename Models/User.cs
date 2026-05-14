namespace shappo0.Models
{
    public enum Roles { Waiter, Bar, Kitchen, Manager}
    public enum Sexe { M, V, X }
    public class User
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string? PhoneNumber { get; set; }
        public Sexe? Sex { get; set; } // M, V, X
        public Roles Role { get; set; } // Waiter, Bar, Kitchen, Manager
        public string Password { get; set; } // 4 cijfers hashed
        public bool IsEmployed { get; set; }

        public User()
        {
        }

        public User(int userId, string userName, string? phoneNumber, Sexe? sex, Roles role, string password, bool isEmployed)
        {
            UserId = userId;
            UserName = userName;
            PhoneNumber = phoneNumber;
            Sex = sex;
            Role = role;
            Password = password;
            IsEmployed = isEmployed;
        }
    }
}
