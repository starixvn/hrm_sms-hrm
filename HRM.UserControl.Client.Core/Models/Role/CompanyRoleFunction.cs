namespace HRM.UserControl.Client.Core.Models.Role
{
    using System.Collections.Generic;

    public class CompanyRoleFunction
    {
        public int CompanyId { get; set; }

        public string CompanyName { get; set; }

        public int RoleId { get; set; }

        public string RoleName { get; set; }

        public IEnumerable<Function> Functions { get; set; }
    }
}