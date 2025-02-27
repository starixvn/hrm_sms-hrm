namespace Fanex.NexHR.Criteria.ManagementLevel
{
    using Fanex.Data.Repository;

    public class InsertManagementLevelCriteria : CriteriaBase
    {
        public string Name { get; set; }

        public int ModifiedBy { get; set; }

        public override string GetSettingKey()
        {
            return "ManagementLevel_Insert";
        }

        public override bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(Name);
        }
    }
}
