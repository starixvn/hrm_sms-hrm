using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nexus.RoleManagement.Services
{
    [Serializable]
    public class SystemRoleModel
    {
        public int RoleId { get; set; }

        public string RoleName { get; set; }

        public string RoleCode { get; set; }
    }
}
