namespace GO.AutoJobs.Criteria.Employee
{
    using Fanex.Data.Repository;

    public class GetEmployeeLastModifiedDateBySiteIdCriteria : CriteriaBase
    {
        public short SiteId { get; set; }

        public override string GetSettingKey() => "AutoJob_Employee_GetLastModifiedDateBySiteId";

        public override bool IsValid() => SiteId > 0;
    }
}