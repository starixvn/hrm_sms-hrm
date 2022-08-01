namespace HRM.UserControl.Client.Core.Session
{
    using System.Collections.Generic;
    using HRM.UserControl.Client.Core.Enumerables;
    using HRM.UserControl.Client.Core.Models;
    using HRM.UserControl.Client.Core.Models.Auth;

    public interface IUserControlSession
    {
        int UserId { get; set; }
        long OnUserId { get; set; }
        short SiteId { get; set; }
        JwtToken JwtToken { get; set; }
        IList<FunctionPermission> Functions { get; set; }
        IList<MenuItem> MenuItems { get; set; }

        T Get<T>(string key);

        void Set<T>(string key, T value);

        bool HasPermissionOnFunction(string functionKey, Policy policy);

        void ClearAll();
    }
}