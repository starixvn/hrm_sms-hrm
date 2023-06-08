namespace GO.Website.Criteria.Manpower.Period
{
    using Fanex.Data;
    using GO.Core.Models;
    using GO.Website.Models.Manpower.Period;
    using System.Collections.Generic;

    public class InsertPeriodCommand : BaseNonQueryCommand
    {
        public int SiteId { get; set; }
        public string Name { get; set; }

        [SpParam(Ignored = true)]
        public IList<PeriodProcessSettingInfo> Settings { get; set; }

        public string SettingXml { get; set; }
        public bool IsActive { get; set; } = true;
        public int CreatedBy { get; set; }

        public override string GetSettingKey() => "Manpower_Period_Insert";

        public override bool IsValid() => !string.IsNullOrWhiteSpace(Name) && CreatedBy > 0;
    }
}
