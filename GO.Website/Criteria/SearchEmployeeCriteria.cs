namespace GO.Website.Criteria
{
    using Fanex.Data.Repository;

    public class SearchEmployeeCriteria : CriteriaBase
    {
        public string Keyword { get; set; }
        public byte Top { get; set; } = 10;

        public override string GetSettingKey() => "HRM_Employee_Search";

        public override bool IsValid() => !string.IsNullOrWhiteSpace(Keyword);
    }
}