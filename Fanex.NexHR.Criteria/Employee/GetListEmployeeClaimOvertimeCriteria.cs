namespace Fanex.NexHR.Criteria.Employee
{
    using Fanex.Data.Repository;

    public class GetListEmployeeClaimOvertimeCriteria : CriteriaBase
    {
        public override string GetSettingKey()
        {
            return "OverTime_GetListEmpClaimOT";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
