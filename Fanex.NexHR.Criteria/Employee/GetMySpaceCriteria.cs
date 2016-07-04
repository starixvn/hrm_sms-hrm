namespace Fanex.NexHR.Criteria.Employee
{
    using System;
    using Fanex.Data.Repository;

    public class GetMySpaceCriteria : CriteriaBase
    {
        public int EmpId { get; set; }

        public DateTime? Date { get; set; }

        public override string GetSettingKey()
        {
            return "Report_EmployeeDashBoard";
        }

        public override bool IsValid()
        {
            return EmpId > 0;
        }
    }
}
