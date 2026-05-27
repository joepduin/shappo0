using shappo0.Models;

namespace shappo0.Repositories
{
    public interface IUsersRepository
    {
        User GetUserByCredentials(LoginModel loginModel);

        User GetUser(int? id);

        List<User> GetAllUsers();

        void AddUser(User user);

        void EditUser(User user);
    }
}
