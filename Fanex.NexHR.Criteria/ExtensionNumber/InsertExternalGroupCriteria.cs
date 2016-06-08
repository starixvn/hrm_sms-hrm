namespace Fanex.NexHR.Criteria.ExtensionNumber
{
    using Fanex.Data.Repository;

    public class InsertExternalGroupCriteria : CriteriaBase
    {
        public string FullName { get; set; }

        public override string GetSettingKey()
        {
            return "ExtensionNO_Insert_GroupExternal";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
