namespace Fanex.NexHR.Core.Enums
{
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;

    [SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue", Justification = "Reviewed. Suppression is OK here.")]
    public enum DisciplineType : int
    {
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
        UnpaidLeaveAbsent = 8,

        [Description("Unplanned Leave")]
        UnplannedLeave = 9,

        [Description("Weekend Leave")]
        WeekendLeave = 10
    }
}