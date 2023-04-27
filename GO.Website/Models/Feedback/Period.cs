namespace GO.Website.Models.Feedback
{
    using System;

    public class Period
    {
        public int PeriodId { get; set; }
        public string Name { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
    }
}