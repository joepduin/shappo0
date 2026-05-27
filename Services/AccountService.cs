using shappo0.Models;
using shappo0.Repositories;
using System.Security.Cryptography;
using System.Text;

namespace shappo0.Services
{
    public class AccountService : IAccountService
    {
        private readonly IStaffRepository _usersRepository;

        public AccountService(IStaffRepository usersRepository)
        {
            _usersRepository = usersRepository;
        }

        public Staff GetStaffByCredentials(LoginModel loginModel)
        {

            loginModel.Password = HashPassword(loginModel.Password);
            return _usersRepository.GetStaffByCredentials(loginModel);
        }

        public Staff GetStaff(int? id)
        {
            return _usersRepository.GetStaff(id);
        }

        public List<Staff> GetAllStaff()
        {
            return _usersRepository.GetAllStaff();
        }

        public void AddStaff(Staff staff)
        {
            Staff copyStaff = new Staff(staff);
            copyStaff.Password = HashPassword(staff.Password);

            _usersRepository.AddStaff(copyStaff);

            if(staff.StaffId != copyStaff.StaffId)
                staff.StaffId = copyStaff.StaffId;
        }

        public void EditStaff(Staff staff)
        {
            _usersRepository.EditStaff(staff);
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
