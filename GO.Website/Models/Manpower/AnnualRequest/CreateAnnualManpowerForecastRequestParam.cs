namespace GO.Website.Models.Manpower
{
    using System.Collections.Generic;

    public class CreateAnnualManpowerForecastRequestParam
    {
        public short PeriodId { get; set; }
        public IList<AnnualManpowerForecastRequestDetail> Details { get; set; }
    }
}