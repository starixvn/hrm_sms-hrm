namespace Fanex.NexHR.Criteria.Department
{
    using Fanex.Data.Repository;

    public class GetDepartmentByIdCriteria : CriteriaBase
    {
        public int DepartmentId { get; set; }

        public override string GetSettingKey() => "Department_GetById";

        public override bool IsValid() => DepartmentId > 0;
    }
}