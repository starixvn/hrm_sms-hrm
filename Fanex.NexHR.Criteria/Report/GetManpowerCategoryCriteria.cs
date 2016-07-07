namespace Fanex.NexHR.Criteria.Report
{
    using Fanex.Data.Repository;

    public class GetManpowerCategoryCriteria : CriteriaBase
    {
        public int Year { get; set; }

        public override string GetSettingKey()
        {
            return "Report_ManpowerCategory";
        }

        public override bool IsValid()
        {
            return Year > 0;
        }
    }
}
