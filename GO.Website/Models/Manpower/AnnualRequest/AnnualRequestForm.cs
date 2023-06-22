namespace GO.Website.Models.Manpower
{
    using System;
    using System.Collections.Generic;
    using GO.Website.Models.Manpower.Period;

    [Serializable]
    public class AnnualRequestForm
    {
        public int RequestId { get; set; }
        public byte StatusId { get; set; }

        public ManpowerPeriod Period { get; set; }

        public IList<AnnualRequestFormDetail> Details { get; set; }
    }
}