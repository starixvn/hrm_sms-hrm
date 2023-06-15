namespace GO.Website.Models.Manpower.Period
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class ManpowerPeriod
    {
        public short PeriodId { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }

        public IList<MapnowerPeriodProcessSetting> Settings { get; set; } = new List<MapnowerPeriodProcessSetting>();
    }
}
