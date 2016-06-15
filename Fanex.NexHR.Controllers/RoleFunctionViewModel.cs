using Nexus.RoleManagement.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fanex.NexHR.Controllers
{
    public class RoleFunctionViewModel
    {
        public int RoleId { get; set; }

        public FunctionModel[] Functions { get; set; }
    }
}
