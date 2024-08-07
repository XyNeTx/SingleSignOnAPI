using Microsoft.EntityFrameworkCore;

namespace SingleSignOnAPI
{
    [PrimaryKey(nameof(EmployeeCode), nameof(DepartmentCode))]
    public class Employee
    {
        public string EmployeeCode { get; set; }
        public string DepartmentCode { get; set; }
        public string? CC { get; set; }
        public string? Class { get; set; }
        public string? Company { get; set; }
        public string? Title { get; set; }
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public string? NameTH { get; set; }
        public string? SurnameTH { get; set; }
        public byte? TypeCode { get; set; }
        public byte? LocationCode { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? CodeResetPassword { get; set; }
        public string? Team { get; set; }
        public int? ResetCountPassword { get; set; }
        public string? Hashtag { get; set; }
        public DateTime? AttendDate { get; set; }
        public DateTime? QuitDate { get; set; }
        public string? Permission { get; set; }
    }
}
