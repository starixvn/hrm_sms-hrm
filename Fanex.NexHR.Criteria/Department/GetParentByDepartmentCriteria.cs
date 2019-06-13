namespace Fanex.NexHR.Criteria.Department
{
    using Fanex.Data.Repository;

    public class GetParentByDepartmentCriteria : CriteriaBase
    {
        public int DepartmentId { get; set; }

        public override string GetSettingKey() => "Department_GetParent";

        public override bool IsValid() => DepartmentId > 0;
    }
}