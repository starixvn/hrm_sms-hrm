namespace HRM.UserControl.Models.Employee
{
    using System;

    public class Employee
    {
        public int EmployeeId { get; set; }
        public string Code { get; set; }
        public string Nickname { get; set; }
        public string FullName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public bool IsEnabled { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsLocal { get; set; }
        public int GroupId { get; set; }
        public int JobTitleId { get; set; }
        public int JobLevelId { get; set; }

        public DateTime? JoinedDate { get; set; }
        public DateTime? ConfirmedDate { get; set; }
        public DateTime? ResignedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}