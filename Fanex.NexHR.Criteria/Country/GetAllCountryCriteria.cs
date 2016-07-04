namespace Fanex.NexHR.Criteria.Country
{
    using Fanex.Data.Repository;

    public class GetAllCountryCriteria : CriteriaBase
    {
        public override string GetSettingKey()
        {
            return "Country_GetAll";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
