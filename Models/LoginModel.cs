namespace shappo0.Models
{
    public class LoginModel
    {
        public string Name { get; set; }
        public string Password { get; set; }

        public LoginModel()
        {
            Name = string.Empty;
            Password = string.Empty;
        }

        public LoginModel(string name, string password)
        {
            Name = name;
            Password = password;
        }
    }
}
