namespace Fanex.NexHR.Criteria.Request
{
    using System;
    using Fanex.Data.Repository;

    public class GetRequestRelatedPeopleCriteria : CriteriaBase
    {
        public long RequestId { get; set; }

        public int Action { get; set; }

        public override string GetSettingKey()
        {
            return "EmployeeSupervisor_GetListSupervisorRequestID";
        }

        public override bool IsValid()
        {
            return RequestId > 0;
        }
    }
}
