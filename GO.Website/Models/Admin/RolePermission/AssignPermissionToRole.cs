namespace GO.Website.Models.Admin.RolePermission
{
    using System;
    using System.Collections.Generic;
    using HRM.UserControl.Client.Core.Models;

    [Serializable]
    public class AssignPermissionToRole
    {
        public short RoleId { get; set; }
        public short SiteId { get; set; }
        public IList<FunctionPermission> Permissions { get; set; }
    }
}
