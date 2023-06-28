namespace GO.Website.Criteria.Manpower
{
    using Fanex.Data.Repository;

    public class GetEmployeeCriteria : CriteriaBase
    {
        public string Keyword { get; set; }
        public short? CompanyId { get; set; }
        public byte? StatusId { get; set; }
        public short PageIndex { get; set; }
        public short? PageSize { get; set; }
        public int TotalRows { get; set; }

        public override string GetSettingKey() => "Manpower_Employee_Get";

        public override bool IsValid() => true;
    }
}