namespace Fanex.NexHR.Core.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using Fanex.NexHR.Core.Constants;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Core.Models;

    public class SessionService : ISessionService
    {
        private readonly HttpContext currentContext;

        public SessionService()
        {
            currentContext = HttpContext.Current;
        }

        public EmployeeSession Employee
        {
            get => (EmployeeSession)currentContext.Session[SessionKey.Employee] ?? new EmployeeSession();
            set => currentContext.Session[SessionKey.Employee] = value;
        }

        public IEnumerable<MenuItemSession> MenuItems
        {
            get => (List<MenuItemSession>)currentContext.Session[SessionKey.MenuItems] ?? new List<MenuItemSession>();
            set => currentContext.Session[SessionKey.MenuItems] = value;
        }

        public IEnumerable<FunctionSession> Functions
        {
            get => (List<FunctionSession>)currentContext.Session[SessionKey.Functions] ?? new List<FunctionSession>();
            set => currentContext.Session[SessionKey.Functions] = value;
        }

        public bool IsShowLeftNavigation
        {
            get => (bool)currentContext.Session[SessionKey.IsShowLeftNavigation];
            set => currentContext.Session[SessionKey.IsShowLeftNavigation] = value;
        }

        public bool HasPermissionOnFunction(string functionKey, Policy policy)
        {
            var result = false;

            if (Functions != null && Functions.Any(x => x.FunctionKey == functionKey))
            {
                switch (policy)
                {
                    case Policy.Read:
                        result = Functions.Any(x => x.FunctionKey == functionKey && (x.IsRead || x.IsHost));
                        break;

                    case Policy.Host:
                        result = Functions.Any(x => x.FunctionKey == functionKey && x.IsHost);
                        break;

                    default:
                        result = false;
                        break;
                }
            }

            return result;
        }

        public void ClearAll()
        {
            currentContext.Session.Abandon();
            currentContext.Session.RemoveAll();
        }
    }
}