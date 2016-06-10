namespace Fanex.NexHR.Core.Enums
{
    using System.Diagnostics.CodeAnalysis;
    using Fanex.Common.Extensions;

    [SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue", Justification = "Reviewed. Suppression is OK here.")]
    public enum RoleSystem : int
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

        [StringValue("EditDiscipline")]
        EditDiscipline = 8
    }
}