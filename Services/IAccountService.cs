using shappo0.Models;

namespace shappo0.Services
{
    public interface IAccountService
    {
        User GetUserByCredentials(LoginModel loginModel);
    }
}
