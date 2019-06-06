namespace Fanex.NexHR.Core.Services
{
    using System.Collections.Generic;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Core.Models;

    public interface ISessionService
    {
        EmployeeSession Employee { get; set; }
        IEnumerable<MenuItemSession> MenuItems { get; set; }
        IEnumerable<FunctionSession> Functions { get; set; }
        bool IsShowLeftNavigation { get; set; }

        bool HasPermissionOnFunction(string functionKey, Policy policy);

        void ClearAll();
    }
}