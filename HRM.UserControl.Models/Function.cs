namespace HRM.UserControl.Models
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class Function
    {
        public int FunctionId { get; set; }
        public string FunctionName { get; set; }
        public string FunctionKey { get; set; }
        public bool IsActive { get; set; }
        public int? ParentId { get; set; }
        public byte? Policy { get; set; }
        public IList<Function> Childs { get; set; }
    }
}