namespace GO.WebAPI.Criteria.UserMapping
{
    using System;
    using Fanex.Data.Repository;

    public class GetUserMappingBySiteIdCriteria : CriteriaBase
    {
        public short SiteId { get; set; }

        public DateTime? LastModifiedDate { get; set; }

        public override string GetSettingKey() => "API_UserMapping_GetBySiteId";

        public override bool IsValid() => SiteId > 0;
    }
}
