namespace Fanex.NexHR.Criteria.Department
{
    using Fanex.Data.Repository;

    public class GetDepartmentByIdCriteria : CriteriaBase
    {
        public int DepartmentId { get; set; }

        public override string GetSettingKey()
        {
            return "Department_GetById";
        }

        public override bool IsValid()
        {
            return DepartmentId > 0;
        }
    }
}
