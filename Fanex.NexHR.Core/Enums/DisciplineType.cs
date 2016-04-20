namespace Fanex.NexHR.Core.Enums
{
    using System.ComponentModel;

    public enum DisciplineType : int
    {
        [Description("None")]
        None = 0,

        [Description("Change Shift")]
        ChangeShift = 1,

        [Description("Missing Check In/Out")]
        MissingCheckInOut = 2,

        [Description("Late In")]
        LateIn = 3,

        [Description("Early Out")]
        EarlyOut = 4,

        [Description("Absent")]
        Absent = 5,

        [Description("Unpaid Leave")]
        UnpaidLeave = 6,

        [Description("Working Under Time")]
        WorkingUnderTime = 7,

        [Description("Unpaid Leave Absent")]
        UnpaidLeaveAbsent = 8
    }
}