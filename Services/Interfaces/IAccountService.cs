using shappo0.Models;
using System.Security.Claims;

namespace shappo0.Services
{
    public interface IAccountService
    {
        public List<Claim> GetUserClaims(Staff staff);

        Staff? GetStaffByCredentials(LoginModel loginModel);

        Staff? GetStaff(int? id);

        List<Staff> GetAllStaff();

        void AddStaff(Staff staff);
        void EditStaff(Staff staff);
    }
}
