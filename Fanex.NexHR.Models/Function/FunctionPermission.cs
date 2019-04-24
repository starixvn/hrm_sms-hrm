namespace Fanex.NexHR.Models.Function
{
    using System;
    using Fanex.NexHR.Core.XMLExtensions;

    [Serializable]
    [XMLName("Row")]
    public class FunctionPermission
    {
        public int FunctionId { get; set; }
        public bool IsRead { get; set; }
        public bool IsHost { get; set; }
    }
}