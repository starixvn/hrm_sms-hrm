namespace Fanex.NexHR.Criteria.NewDepartment
{
    using Fanex.Data.Repository;

    public class GetDepartmentByActiveCriteria : CriteriaBase
    {
        public bool? IsActive { get; set; }

        public override string GetSettingKey() => "Department_GetByActive";

        public override bool IsValid() => true;
    }
}