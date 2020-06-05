namespace Fanex.NexHR.Criteria.Section
{
    using Fanex.Data.Repository;

    public class GetSectionByParentIdCriteria : CriteriaBase
    {
        public int? ParentId { get; set; }
        public bool? IsActive { get; set; }

        public override string GetSettingKey() => "Section_GetByParentId";

        public override bool IsValid() => true;
    }
}
