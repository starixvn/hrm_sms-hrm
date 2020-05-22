namespace Fanex.NexHR.Criteria.NewDepartment
{
    using Fanex.Data.Repository;

    public class GetDepartmentByIdCriteria : CriteriaBase
    {
        public int Id { get; set; }

        public override string GetSettingKey() => "TestDepartment_GetById";

        public override bool IsValid() => Id > 0;
    }
}