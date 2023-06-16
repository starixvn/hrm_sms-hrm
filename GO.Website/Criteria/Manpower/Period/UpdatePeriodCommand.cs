namespace GO.Website.Criteria.Manpower.Period
{
    using Fanex.Data;
    using GO.Core.Models;
    using GO.Website.Models.Manpower.Period;
    using System.Collections.Generic;

    public class UpdatePeriodCommand : BaseNonQueryCommand
    {
        public short PeriodId { get; set; }
        public string Name { get; set; }

        [SpParam(Ignored = true)]
        public IList<MapnowerPeriodProcessSetting> Settings { get; set; }

        public string SettingXml { get; set; }
        public int ModifiedBy { get; set; }

        public override string GetSettingKey() => "Manpower_Period_Update";

        public override bool IsValid() => PeriodId > 0 && !string.IsNullOrWhiteSpace(Name) && ModifiedBy > 0;
    }
}