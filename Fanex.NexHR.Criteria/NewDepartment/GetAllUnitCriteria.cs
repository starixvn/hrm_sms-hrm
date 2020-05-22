namespace Fanex.NexHR.Criteria.NewDepartment
{
    using Fanex.Data.Repository;

    public class GetAllUnitCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "TestDepartment_GetAllUnit";

        public override bool IsValid() => true;
    }
}
