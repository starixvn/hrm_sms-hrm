namespace Fanex.NexHR.Models.Request
{
    using System;
    using System.Collections.Generic;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Resources;

    public class LeaveDisciplineModel
    {
        public DateTime WorkDate { get; set; }

        public bool IsOffDayOrHoliday { get; set; }

        public string ScheduleStatus { get; set; }

        public bool IsUnpaidLeaveAbsent { get; set; }

        public bool IsUnpaidLeave { get; set; }

        public bool IsAbsent { get; set; }

        public bool IsUnplannedLeave { get; set; }

        public bool IsWeekendLeave { get; set; }

        public IList<LeaveDisciplineDetailModel> LeaveDetail { get; set; }

        public int DisciplineTypeId
        {
            get
            {
                int disciplineTypeId;
                if (IsUnpaidLeaveAbsent)
                {
                    disciplineTypeId = (int)DisciplineType.UnpaidLeaveAbsent;
                }
                else if (IsAbsent)
                {
                    disciplineTypeId = (int)DisciplineType.Absent;
                }
                else if (IsUnplannedLeave)
                {
                    disciplineTypeId = (int)DisciplineType.UnplannedLeave;
                }
                else if (IsUnpaidLeave)
                {
                    disciplineTypeId = (int)DisciplineType.UnpaidLeave;
                }
                else
                {
                    disciplineTypeId = -1;
                }

                return disciplineTypeId;
            }
        }

        public string DisciplineTypeName
        {
            get
            {
                string disciplineName;
                if (IsUnpaidLeaveAbsent)
                {
                    disciplineName = Labels.UnpaidLeaveAbsent;
                }
                else if (IsAbsent)
                {
                    disciplineName = Labels.Absent;
                }
                else if (IsUnplannedLeave)
                {
                    disciplineName = Labels.LateRequestLeave;
                }
                else if (IsUnpaidLeave)
                {
                    disciplineName = Labels.UnpaidLeave;
                }
                else
                {
                    disciplineName = string.Empty;
                }

                return disciplineName;
            }
        }
    }
}
