namespace Fanex.NexHR.Repositories
{
    using System;
    using System.Collections.Generic;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.NexHR.Models.Report.Overtime;

    public class OvertimeReportRepository : IOvertimeReportRepository
    {
        public Tuple<IEnumerable<DateRange>, IEnumerable<DailyOvertimeDetails>> GetMonthlyOvertime(ICriteria criteria)
        {
            using (IObjectDb db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (var multi = db.QueryMultiple(criteria.GetSpParams()))
                {
                    var dateRanges = multi.Read<DateRange>();
                    var dailyOvertimes = multi.Read<DailyOvertimeDetails>();

                    return new Tuple<IEnumerable<DateRange>, IEnumerable<DailyOvertimeDetails>>(dateRanges, dailyOvertimes);
                }
            }
        }
    }
}