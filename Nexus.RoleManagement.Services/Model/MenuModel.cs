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

        public string MenuName { get; set; }

        public string MenuLink { get; set; }

        public int? ParentId { get; set; }

        public int FunctionId { get; set; }

        public int DisplayOrder { get; set; }

        public bool IsActive { get; set; }
    }
}
