namespace HRM.UserControl.Models
{
    using System.Collections.Generic;

    public class CompanyRole
    {
        public int RoleId { get; set; }

        public string RoleName { get; set; }

        public int CompanyId { get; set; }

        public string CompanyName { get; set; }

        public IEnumerable<Function> Functions { get; set; }
    }
}