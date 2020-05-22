namespace Fanex.NexHR.Criteria.NewDepartment
{
    using Fanex.Data.Repository;

    public class DeleteDepartmentCriteria : CriteriaBase
    {
        public int departmentId { get; set; }

        public override string GetSettingKey()
        {
            return "TestDepartment_Delete";
        }

        public override bool IsValid()
        {
            return departmentId > 0;
        }
    }
}

