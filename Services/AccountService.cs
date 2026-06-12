using shappo0.Models;
using shappo0.Repositories;
using System.Security.Cryptography;
using System.Text;

namespace shappo0.Services
{
    public class AccountService : IAccountService
    {
        private readonly IStaffRepository _staffRepository;

        public AccountService(IStaffRepository staffRepository)
        {
            _staffRepository = staffRepository;
        }

        // Joep
        public Staff? GetStaffByCredentials(LoginModel loginModel)
        {

            loginModel.Password = HashPassword(loginModel.Password);
            return _staffRepository.GetStaffByCredentials(loginModel);
        }

        // Ylvi
        public Staff? GetStaff(int? id)
        {
            return _staffRepository.GetStaff(id);
        }

        public List<Staff> GetAllStaff()
        {
            return _staffRepository.GetAllStaff();
        }

        public void AddStaff(Staff staff)
        {
            //Creates a copy user object to prevent original user not loading anymore in case of error
            Staff copyStaff = new Staff(staff);

            copyStaff.Password = HashPassword(staff.Password);
            _staffRepository.AddStaff(copyStaff);

            if(staff.StaffId != copyStaff.StaffId)
                staff.StaffId = copyStaff.StaffId;
        }

        public void EditStaff(Staff staff)
        {
            _staffRepository.EditStaff(staff);
        }

        // Joep
        //maakt een hash van het wachtwoord
        private string HashPassword(string? password)
        {
            if(password == null)
                return string.Empty;

            using (SHA256 sha256 = SHA256.Create())
            {
                return Convert.ToBase64String(sha256.ComputeHash(Encoding.UTF8.GetBytes(password)));
            }
        }
    }
}
