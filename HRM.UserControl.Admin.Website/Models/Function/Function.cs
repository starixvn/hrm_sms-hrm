namespace HRM.UserControl.Admin.Website.Models.Function
{
    using System;
    using System.Collections.Generic;

    public class Function
    {
        public int FunctionId { get; set; }
        public string FunctionName { get; set; }
        public string FunctionKey { get; set; }
        public short SiteId { get; set; }
        public int ParentId { get; set; }
        public short? Policy { get; set; }
        public short DisplayOrder { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public IEnumerable<Function> Children { get; set; }
    }
}
