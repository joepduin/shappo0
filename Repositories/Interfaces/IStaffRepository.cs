using shappo0.Models;

namespace shappo0.Repositories
{
    public interface IStaffRepository
    {
        Staff? GetStaffByCredentials(LoginModel loginModel);

        Staff? GetStaff(int? id);

        List<Staff> GetAllStaff();

        void AddStaff(Staff staff);

        void EditStaff(Staff staff);
    }
}
