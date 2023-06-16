namespace GO.Website.Criteria.Manpower.Period
{
    using Fanex.Data;
    using GO.Core.Models;
    using GO.Website.Constants;
    using GO.Website.Models.Manpower.Period;
    using System.Collections.Generic;
    using System.Linq;

    public class CreatePeriodCommand : BaseNonQueryCommand
    {
        public string Name { get; set; }

        [SpParam(Ignored = true)]
        public IList<MapnowerPeriodProcessSetting> Settings { get; set; }

        public string SettingXml { get; set; }
        public bool IsActive { get; set; } = true;
        public int CreatedBy { get; set; }

        public override string GetSettingKey() => "Manpower_Period_Insert";

        public override bool IsValid() => !string.IsNullOrWhiteSpace(Name) && CreatedBy > 0 && IsCheckSettings();
        private bool IsCheckSettings()
        {
            bool checkSetting;
            var unitHead = Settings.Where(x => x.ProcessId == ManpowerPeriodProcessSetting.UnitHead).First();
            var verifier = Settings.Where(x => x.ProcessId == ManpowerPeriodProcessSetting.Verifier).First();
            var approver = Settings.Where(x => x.ProcessId == ManpowerPeriodProcessSetting.Approver).First();

            if (unitHead.FromDate > unitHead.ToDate || verifier.FromDate > verifier.ToDate || approver.FromDate > approver.ToDate)
            {
                checkSetting = false;
            }
            else if (verifier.FromDate < unitHead.ToDate || verifier.ToDate < unitHead.ToDate)
            {
                checkSetting = false;
            }
            else if (approver.FromDate < verifier.ToDate || approver.ToDate < verifier.ToDate)
            {
                checkSetting = false;
            }
            else { checkSetting = true; }
            return checkSetting;
        }
    }
}