using shappo0.Models;
using shappo0.Repositories;
using System.Security.Cryptography;
using System.Text;

namespace shappo0.Services
{
    public class AccountService : IAccountService
    {
        private readonly IUsersRepository _usersRepository;

        public AccountService(IUsersRepository usersRepository)
        {
            _usersRepository = usersRepository;
        }

        public User GetUserByCredentials(LoginModel loginModel)
        {

            loginModel.Password = HashPassword(loginModel.Password);
            return _usersRepository.GetUserByCredentials(loginModel);
        }

        public User GetUser(int? id)
        {
            return _usersRepository.GetUser(id);
        }

        public List<User> GetAllUsers()
        {
            return _usersRepository.GetAllUsers();
        }

        public void AddUser(User user)
        {
            User copyUser = new User(user);
            copyUser.Password = HashPassword(user.Password);

            _usersRepository.AddUser(copyUser);

            if(user.UserId != copyUser.UserId)
                user.UserId = copyUser.UserId;
        }

        public void EditUser(User user)
        {
            _usersRepository.EditUser(user);
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
