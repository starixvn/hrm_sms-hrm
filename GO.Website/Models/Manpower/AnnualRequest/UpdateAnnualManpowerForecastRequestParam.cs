namespace GO.Website.Models.Manpower.AnnualRequest
{
    using System.Collections.Generic;

    public class UpdateAnnualManpowerForecastRequestParam
    {
        public int RequestId { get; set; }
        public byte StatusId { get; set; }
        public short PeriodId { get; set; }
        public string UnitName { get; set; }
        public IList<AnnualManpowerForecastRequestDetail> Details { get; set; }
    }
}
