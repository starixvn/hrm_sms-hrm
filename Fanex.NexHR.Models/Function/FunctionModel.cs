namespace Fanex.NexHR.Models
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class FunctionModel
    {
        public int FunctionId { get; set; }
        public string FunctionKey { get; set; }
        public string Name { get; set; }
        public int? ParentFunctionId { get; set; }
        public int Policy { get; set; }
        public bool IsRead { get; set; }
        public bool IsHost { get; set; }
        public int Level { get; set; }
        public IList<FunctionModel> Childs { get; set; } = new List<FunctionModel>();
    }
}