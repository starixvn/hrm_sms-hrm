namespace Fanex.NexHR.Core.Models
{
    using System;

    [Serializable]
    public class FunctionSession
    {
        public int FunctionId { get; set; }
        public string FunctionKey { get; set; }
        public string Name { get; set; }
        public bool IsRead { get; set; }
        public bool IsHost { get; set; }
    }
}