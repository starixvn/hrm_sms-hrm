namespace HRM.UserControl.Admin.Website.Models
{
    using System.Collections.Generic;
    using HRM.UserControl.Client.Core.Models;

    public class AssignPermissionToRole
    {
        public short RoleId { get; set; }
        public short SiteId { get; set; }
        public IList<FunctionPermission> Permissions { get; set; }
    }
}