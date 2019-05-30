namespace Fanex.NexHR.Core.Enums
{
    using Fanex.Common.Extensions;

    public enum RoleSystem
    {
        [StringValue("Member")]
        Member = 1,

        [StringValue("Verifier")]
        Verifier = 2,

        [StringValue("Approver")]
        Approver = 3,

        [StringValue("ShiftManager")]
        ShiftManager = 4,

        [StringValue("ActingSupervisor")]
        ActingSupervisor = 5,

        [StringValue("HR")]
        HR = 6,

        [StringValue("SystemAdmin")]
        SystemAdmin = 7,

        [StringValue("DisciplineManager")]
        DisciplineManager = 8
    }
}