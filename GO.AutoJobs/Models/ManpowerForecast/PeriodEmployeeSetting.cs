namespace GO.AutoJobs.Models.ManpowerForecast
{
    using System;

    public class PeriodEmployeeSetting
    {
        public int EmpId { get; set; }
        public string Nickname { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }

        public short PeriodId { get; set; }
        public byte ProcessId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
    }
}