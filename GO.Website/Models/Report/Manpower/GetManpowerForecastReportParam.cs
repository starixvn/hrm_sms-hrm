namespace GO.Website.Models.Report.Manpower
{
    public class GetManpowerForecastReportParam
    {
        public int UserId { get; set; }
        public short SiteId { get; set; }
        public short PeriodId { get; set; }
        public string UnitType { get; set; }
        public int Year { get; set; }
        public string PeriodName { get; set; }
    }
}