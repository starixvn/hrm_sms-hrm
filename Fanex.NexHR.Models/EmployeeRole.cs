using Nexus.RoleManagement.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fanex.NexHR.Models
{
    public class EmployeeRole
    {
        public int EmployeeId { get; set; }

        public SystemRoleModel Role { get; set; }
    }
}
