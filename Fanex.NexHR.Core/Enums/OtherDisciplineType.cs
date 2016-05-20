namespace Fanex.NexHR.Core.Enums
{
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;

    [SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue", Justification = "Reviewed. Suppression is OK here.")]
    public enum OtherDisciplineType : int
    {
        [Description("Verbal Warning")]
        VerbalWarning = -1,

        [Description("Written Warning")]
        WrittenWarning = -2
    }
}