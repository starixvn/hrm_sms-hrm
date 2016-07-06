namespace Fanex.NexHR.Criteria.Supervisor
{
    using System;
    using Fanex.Data.Repository;

    public class GetEmployeeSupervisorCriteria : CriteriaBase
    {
        public int EmpId { get; set; }

        public int Role { get; set; }

        public override string GetSettingKey()
        {
            return "EmployeeSupervisor_GetRequestByEmpID";
        }

        public override bool IsValid()
        {
            return EmpId > 0;
        }
    }
}
