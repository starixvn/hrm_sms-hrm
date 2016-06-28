using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nexus.RoleManagement.Services
{
    public class MenuViewModel
    {
        public int MenuId { get; set; }

        public string Name { get; set; }

        public string Text { get; set; }

        public string Link { get; set; }

        public int DisplayOrder { get; set; }

        public bool IsActive { get; set; }

        public int Policy { get; set; }

        public FunctionModel Function { get; set; }

        public MenuModel ParentMenu { get; set; }
    }
}
