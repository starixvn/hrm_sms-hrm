namespace Fanex.NexHR.Criteria.Lookup
{
    using Fanex.Data.Repository;

    public class GetAllLookupDataCriteria : CriteriaBase
    {
        public string LookupCode { get; set; }

        public override string GetSettingKey()
        {
            return "Lookup_GetLookupData";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
