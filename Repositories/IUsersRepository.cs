using shappo0.Models;

namespace shappo0.Repositories
{
    public interface IUsersRepository
    {
        User GetUserByCredentials(LoginModel loginModel);
    }
}
