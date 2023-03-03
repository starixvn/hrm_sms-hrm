namespace HRM.UserControl.WebAPI.Criteria.UnitHead
{
    using Fanex.Data.Repository;

    public class GetUnitHeadInfoCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "API_UnitHead_GetInfo";

        public override bool IsValid() => true;
    }
}