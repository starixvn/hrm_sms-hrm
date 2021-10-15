namespace HRM.UserControl.Client.Core.Models
{
    using System.Collections.Generic;

    public class Function
    {
        public int RoleId { get; set; }

        public int FunctionId { get; set; }

        public string FunctionName { get; set; }

        public string FunctionKey { get; set; }

        public string Description { get; set; }

        public bool IsReadonly { get; set; }

        public bool IsActive { get; set; }

        public int? ParentId { get; set; }

        public IEnumerable<Function> ChildFunctions { get; set; }
    }
}