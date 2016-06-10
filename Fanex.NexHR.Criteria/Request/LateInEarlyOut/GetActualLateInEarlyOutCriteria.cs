namespace Fanex.NexHR.Criteria.Request.LateInEarlyOut
{
    using System;
    using Fanex.Data.Repository;

    public class GetActualLateInEarlyOutCriteria : CriteriaBase
    {
        public int EmployeeId { get; set; }

        public DateTime WorkDate { get; set; }

        public override string GetSettingKey()
        {
            return "TimeSheet_GetLateInEarlyOut";
        }

        public override bool IsValid()
        {
            return EmployeeId > 0;
        }
    }
}
