namespace Fanex.NexHR.Criteria.ExtensionNumber
{
    using Fanex.Data.Repository;

    public class GetExtensionNumberGroupInfoCriteria : CriteriaBase
    {
        public bool? IsInternal { get; set; }

        public int? GroupID { get; set; }

        public override string GetSettingKey()
        {
            return "ExtensionNO_GetGroupInfo";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
