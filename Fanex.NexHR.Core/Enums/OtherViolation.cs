namespace Fanex.NexHR.Core.Enums
{
    using System.ComponentModel;

    public enum OtherViolation : int
    {
        [Description("None")]
        None = 0,

        [Description("Verbal Warning")]
        VerbalWarning = -1,

        [Description("Written Warning")]
        WrittenWarning = -2
    }
}