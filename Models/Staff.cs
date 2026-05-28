using shappo0.Enums;

namespace shappo0.Models
{
    public class Staff
    {
        public int StaffId { get; set; }
        public string Name { get; set; }
        public string? PhoneNumber { get; set; }
        public SexType? Sex { get; set; } // M, V, X
        public RoleType Role { get; set; } // Waiter, Bar, Kitchen, Manager
        public string Password { get; set; } // 4 cijfers hashed
        public bool IsEmployed { get; set; }

        public Staff()
        {
        }

        public Staff (Staff staff)
        {
            StaffId = staff.StaffId;
            Name = staff.Name;
            PhoneNumber = staff.PhoneNumber;
            Sex = staff.Sex;
            Role = staff.Role;
            Password = staff.Password;
            IsEmployed = staff.IsEmployed;
        }

        public Staff(int staffId, string name, string? phoneNumber, SexType? sex, RoleType role, string password, bool isEmployed)
        {
            StaffId = staffId;
            Name = name;
            PhoneNumber = phoneNumber;
            Sex = sex;
            Role = role;
            Password = password;
            IsEmployed = isEmployed;
        }
    }
}
