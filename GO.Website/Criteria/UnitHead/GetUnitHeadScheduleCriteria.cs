namespace GO.Website.Criteria.UnitHead
{
    using System;
    using Fanex.Data.Repository;

    public class GetUnitHeadScheduleCriteria : CriteriaBase
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }

        public override string GetSettingKey() => "API_UnitHead_GetSchedule";

        public override bool IsValid() => true;
    }
}