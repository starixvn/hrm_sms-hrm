namespace HRM.UserControl.Client.Core.Session
{
    using System.Collections.Generic;
    using HRM.UserControl.Client.Core.Enumerables;
    using HRM.UserControl.Client.Core.Models;

    public interface IUserControlSession
    {
        int UserId { get; set; }
        string Nickname { get; set; }
        short SiteId { get; set; }
        string JwtToken { get; set; }

        IList<FunctionPermission> Functions { get; set; }

        IList<MenuItem> MenuItems { get; set; }

        bool HasPermissionOnFunction(string functionKey, Policy policy);

        void ClearAll();
    }
}