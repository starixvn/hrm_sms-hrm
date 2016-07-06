namespace Fanex.NexHR.Criteria.Country
{
    using System;
    using Fanex.Data.Repository;

    public class GetCountryByCodeCriteria : CriteriaBase
    {
        public string CountryCode { get; set; }

        public override string GetSettingKey()
        {
            return "Country_GetByCode";
        }

        public override bool IsValid()
        {
            return !string.IsNullOrEmpty(CountryCode);
        }
    }
}
