namespace Fanex.NexHR.Models
{
    using System;

    [Serializable]
    public class FunctionModel
    {
        public int FunctionId { get; set; }

        public string FunctionKey { get; set; }

        public string Name { get; set; }

        public int? ParentFunctionId { get; set; }

        public bool IsReadOnly { get; set; }

        public bool IsRead { get; set; }
        public bool IsHost { get; set; }
    }
}