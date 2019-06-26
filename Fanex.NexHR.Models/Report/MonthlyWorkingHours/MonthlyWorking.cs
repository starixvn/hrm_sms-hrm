namespace Fanex.NexHR.Models.Report.MonthlyWorkingHours
{
    using System.Collections.Generic;

    public class MonthlyWorking
    {
        public int EmployeeId { get; set; }
        public string Code { get; set; }
        public string Nickname { get; set; }
        public string FullName { get; set; }
        public int WorkingDays { get; set; }

        public decimal ActualWorkingDays
        {
            get
            {
                decimal workingDays = 0;

                foreach (DailyWorking dailyWorking in DailyWorkings)
                {
                    if (dailyWorking.IsLeaveDay)
                    {
                        workingDays += dailyWorking.IsLeaveHalfDay && dailyWorking.TotalWorkingHours > 0 ? 0.5M : 0;
                    }
                    else
                    {
                        workingDays += dailyWorking.TotalWorkingHours > 0 ? 1 : 0;
                    }
                }

                return workingDays;
            }
        }

        public EntityBase<int> Section { get; set; }
        public IEnumerable<DailyWorking> DailyWorkings { get; set; }
    }
}