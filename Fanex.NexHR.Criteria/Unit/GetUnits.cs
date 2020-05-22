namespace Fanex.NexHR.Criteria.Unit
{
    using Fanex.Data.Repository;

    public class GetUnits : CriteriaBase
    {
        public bool? IsActive { get; set; }

        public override string GetSettingKey() => "Unit_Get";

        public override bool IsValid() => true;
    }
}
