namespace Fanex.NexHR.Criteria.Department
{
    using Fanex.Data.Repository;

    public class GetDepartmentByParentIdCriteria : CriteriaBase
    {
        public int? ParentId { get; set; }
        public bool? IsActive { get; set; }

        public override string GetSettingKey() => "Department_GetByParentId";

        public override bool IsValid() => true;
    }
}
