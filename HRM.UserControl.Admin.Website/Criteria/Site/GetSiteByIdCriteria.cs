

namespace HRM.UserControl.Admin.Website.Criteria.Site
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Fanex.Data.Repository;

    public class GetSiteByIdCriteria : CriteriaBase
    {
        public short SiteId { get; set; }

        public override string GetSettingKey() => "Site_GetById";

        public override bool IsValid() => SiteId > 0;
    }
}
