using shappo0.Models;
using shappo0.Repositories;

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
            return _usersRepository.GetUserByCredentials(loginModel);
        }
    }
}
