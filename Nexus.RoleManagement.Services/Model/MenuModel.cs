using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nexus.RoleManagement.Services
{
    [Serializable]
    public class MenuModel
    {
        public int MenuId { get; set; }

        public string Name { get; set; }

        public string Text { get; set; }

        public string Link { get; set; }
        
        public int? ParentId { get; set; }

        public int? FunctionId { get; set; }

        public int DisplayOrder { get; set; }

        public bool IsActive { get; set; }
    }
}
