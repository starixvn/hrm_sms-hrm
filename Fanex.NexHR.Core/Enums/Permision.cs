namespace Fanex.NexHR.Core.Enums
{
    using System.ComponentModel;

    public enum Permision : int
    {
        [Description("None")]
        None = 0,

        [Description("Read")]
        Read = 1,

        [Description("Host")]
        Host = 2
    }
}