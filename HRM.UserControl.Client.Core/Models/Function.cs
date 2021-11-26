namespace HRM.UserControl.Client.Core.Models
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class Function
    {
        public int FunctionId { get; set; }
        public string FunctionName { get; set; }
        public string FunctionKey { get; set; }
        public short SiteId { get; set; }
        public bool IsActive { get; set; }
        public int? ParentId { get; set; }
        public byte? Policy { get; set; }
        public byte Level { get; set; }
        public int DisplayOrder { get; set; }
        public string Description { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public IList<Function> Childs { get; set; } = new List<Function>();
    }
}