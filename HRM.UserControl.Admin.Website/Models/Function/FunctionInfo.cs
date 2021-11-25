namespace HRM.UserControl.Admin.Website.Models.Function
{
    using System.Collections.Generic;
    public class FunctionInfo
    {
        public int Id { get; set; }
        public Function ParentInfo { get; set; }
        public IEnumerable<Function> ChildInfo { get; set; }
    }
}
