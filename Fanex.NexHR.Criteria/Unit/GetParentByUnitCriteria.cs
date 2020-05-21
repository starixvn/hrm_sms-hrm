namespace Fanex.NexHR.Criteria.Unit
{
    using Fanex.Data.Repository;

    public class GetParentByUnitCriteria : CriteriaBase
    {
        public int DepartmentId { get; set; }

        public override string GetSettingKey() => "Department_GetParent";

        public override bool IsValid() => DepartmentId > 0;
    }
}