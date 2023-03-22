namespace GO.AutoJobs.Models.UnitHead
{
    using System;
    using GO.Core.Extensions.XML;

    [Serializable]

    [XMLName("Row")]
    public class UnitHeadDailySchedule
    {
        public short CompanyId { get; set; }
        public short SiteId { get; set; }

        public int EmpId { get; set; }
        public DateTime WorkDate { get; set; }
        public bool IsWorkDay { get; set; }
        public bool IsOffDay { get; set; }
        public bool IsHoliday { get; set; }
        public bool IsLeaveDay { get; set; }

        public string LeaveTypeCode { get; set; }
        public string LeaveTypeName { get; set; }
    }
}