namespace Fanex.NexHR.Criteria.Employee
{
    using Fanex.Data.Repository;
    using Fanex.NexHR.Core.Attributes;
    using System;

    public class InsertEmployeeCriteria : CriteriaBase
    {
        [Output]
        public int EmpId { get; set; }

        public string FullName { get; set; }

        public string NickName { get; set; }

        public string DomainUserName { get; set; }

        public string Code { get; set; }

        public int? MonShift { get; set; }

        public int? TueShift { get; set; }

        public int? WedShift { get; set; }

        public int? ThuShift { get; set; }

        public int? FriShift { get; set; }

        public int? SatShift { get; set; }

        public int? SunShift { get; set; }

        public int MainGroupId { get; set; }

        public DateTime CreatedDate { get; set; }

        public int CreateBy { get; set; }

        public string Password { get; set; }

        public bool IsDefaultSchedule { get; set; }

        public override string GetSettingKey()
        {
            return "Employee_Information_Insert";
        }

        public override bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(Code) && !string.IsNullOrWhiteSpace(NickName);
        }
    }
}
