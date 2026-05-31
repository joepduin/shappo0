namespace shappo0.Models
{
    public class LoginModel
    {
        public string Name { get; set; }
        public string Password { get; set; }

        public LoginModel()
        {
        }

        public LoginModel(string name, string password)
        {
            Name = name;
            Password = password;
        }
    }
}
