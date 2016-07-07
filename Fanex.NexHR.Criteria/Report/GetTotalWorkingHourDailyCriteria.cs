namespace Fanex.NexHR.Criteria
{
    using System;
    using Fanex.Data.Repository;

    public class GetTotalWorkingHourDailyCriteria : CriteriaBase
    {
        public DateTime WorkDate { get; set; }

        public int? EmpId { get; set; }

        public string GroupIds { get; set; }

        public byte? IsLocal { get; set; }

        public override string GetSettingKey()
        {
            return "Report_EmployeeTotalWorkingHourByDay";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}