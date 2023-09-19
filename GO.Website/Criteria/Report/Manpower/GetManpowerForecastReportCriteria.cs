namespace GO.Website.Criteria.Report.Manpower
{
    using System.Xml.Serialization;
    using Fanex.Data.Repository;

    public class GetManpowerForecastReportCriteria : CriteriaBase
    {
        public int UserId { get; set; }
        public int SiteId { get; set; }
        public int PeriodId { get; set; }
        public string UnitType { get; set; }
        public override string GetSettingKey() => "Report_ManpowerForecast_Get";

        public override bool IsValid() => UserId > 0 && SiteId > 0 && PeriodId > 0;
    }
}
