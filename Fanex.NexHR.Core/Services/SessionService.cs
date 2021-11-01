namespace Fanex.NexHR.Core.Services
{
    using System.Web;
    using Fanex.NexHR.Core.Constants;
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

        public bool IsShowLeftNavigation
        {
            get => (bool)currentContext.Session[SessionKey.IsShowLeftNavigation];
            set => currentContext.Session[SessionKey.IsShowLeftNavigation] = value;
        }

        public void ClearAll()
        {
            currentContext.Session.Abandon();
            currentContext.Session.RemoveAll();
        }
    }
}