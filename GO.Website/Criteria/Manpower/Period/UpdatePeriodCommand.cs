namespace GO.Website.Criteria.Manpower.Period
{
    using Fanex.Data;
    using GO.Core.Models;
    using GO.Website.Models.Manpower.Period;
    using System.Collections.Generic;

    public class UpdatePeriodCommand : BaseNonQueryCommand
    {
        public int PeriodId { get; set; }
        public int SiteId { get; set; }
        public string Name { get; set; }

        [SpParam(Ignored = true)]
        public IList<PeriodProcessSettingInfo> Settings { get; set; }

        public string SettingXml { get; set; }
        public int ModifiedBy { get; set; }

        public override string GetSettingKey() => "Manpower_Period_Update";
        public override bool IsValid() => PeriodId > 0 && ModifiedBy > 0;
    }
}
