using shappo0.Models;

namespace shappo0.Services
{
    public interface IAccountService
    {
        User GetUserByCredentials(LoginModel loginModel);

        User GetUser(int? id);

        List<User> GetAllUsers();

        void AddUser(User user);
        void EditUser(User user);
    }
}
