using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nexus.RoleManagement.Services
{
    [Serializable]
    public class FunctionModel
    {
        public int FunctionId { get; set; }

        public string FunctionKey { get; set; }

        public string Name { get; set; }

        public int? ParentFunctionId { get; set; }

        public bool IsRead { get; set; }

        public bool IsEdit { get; set; }

        public bool IsDelete { get; set; }

        public bool IsCreate { get; set; }
    }
}
