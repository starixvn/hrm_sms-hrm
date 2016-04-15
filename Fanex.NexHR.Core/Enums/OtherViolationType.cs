namespace Fanex.NexHR.Core.Enums
{
    using System.ComponentModel;

    public enum OtherViolationType : int
    {
        [Description("None")]
        None = 0,

        [Description("Verbal Warning")]
        VerbalWarning = -1,

        [Description("Written Warning")]
        WrittenWarning = -2
    }
}