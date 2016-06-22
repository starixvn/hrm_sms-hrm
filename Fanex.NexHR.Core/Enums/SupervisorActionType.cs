namespace Fanex.NexHR.Core.Enums
{
    using System.Diagnostics.CodeAnalysis;
    using Fanex.Common.Extensions;

    [SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue", Justification = "Reviewed. Suppression is OK here.")]
    public enum SupervisorActionType
    {
        [StringValue("Verified by")]
        Verifier = 2,

        [StringValue("Approved by")]
        Approver = 3,

        [StringValue("Shift Arrangement")]
        ShiftArrangement = 4,

        [StringValue("On Behalf Request")]
        OnBehalfRequest = 5,

        [StringValue("Discipline Manager")]
        DisciplineManager = 8
    }
}