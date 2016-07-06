namespace Fanex.NexHR.Criteria.Employee
{
    using System;
    using Fanex.Data.Repository;

    public class GetHRLeaderDashboardCriteria : CriteriaBase
    {
        public int Empid { get; set; }

        public DateTime Date { get; set; }

        public override string GetSettingKey()
        {
            return "Report_HRLeaderDashBoard";
        }

        public override bool IsValid()
        {
            return Empid > 0;
        }
    }
}
