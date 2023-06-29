namespace GO.Website.Criteria.Manpower
{
    using Fanex.Data.Repository;

    public class GetUnitByUserIdCriteria : CriteriaBase
    {
        public int UserId { get; set; }

        public override string GetSettingKey() => "Manpower_Unit_GetByUserId";

        public override bool IsValid() => UserId > 0;
    }
}