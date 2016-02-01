namespace Fanex.NexHR.Criteria.Employee
{
    using System;
    using Fanex.Data.Repository;

    public class GetMemberByManagerCriteria : CriteriaBase
    {
        public string Query { get; set; }

        public int SupervisorId { get; set; }

        public bool ExcludedManager { get; set; }

        public override string GetSettingKey()
        {
            return "Employee_GetMemberByManager";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
