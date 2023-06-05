namespace GO.Website.Models.Manpower.Period
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class PeriodInfo
    {
        public int RowNum { get; set; }
        public int PeriodId { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }

        public IList<PeriodProcessSettingInfo> Settings { get; set; } = new List<PeriodProcessSettingInfo>();
    }
}
