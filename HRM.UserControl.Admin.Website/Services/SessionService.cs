namespace HRM.UserControl.Admin.Website.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using HRM.UserControl.Admin.Website.Constants;
    using HRM.UserControl.Admin.Website.Enumerations;
    using HRM.UserControl.Admin.Website.Extensions;
    using HRM.UserControl.Admin.Website.Models.Session;
    using Microsoft.AspNetCore.Http;

    public interface ISessionService
    {
        UserSession User { get; set; }

        IEnumerable<MenuItemSession> MenuItems { get; set; }

        IEnumerable<FunctionSession> Functions { get; set; }

        bool HasPermissionOnFunction(string functionKey, Policy policy);

        void ClearAll();
    }

    public class SessionService : ISessionService
    {
        private readonly ISession session;

        public SessionService(ISession session)
        {
            this.session = session;
        }

        public SessionService(IHttpContextAccessor accessor)
        {
            session = accessor.HttpContext.Session;
        }

        public UserSession User
        {
            get => session.Get<UserSession>(SessionKey.InternalUser);
            set => session.Set(SessionKey.InternalUser, value);
        }

        public IEnumerable<MenuItemSession> MenuItems
        {
            get => session.Get<List<MenuItemSession>>(SessionKey.InternalMenuItems);
            set => session.Set(SessionKey.InternalMenuItems, value);
        }

        public IEnumerable<FunctionSession> Functions
        {
            get => session.Get<List<FunctionSession>>(SessionKey.InternalFunctions);
            set => session.Set(SessionKey.InternalFunctions, value);
        }

        public bool HasPermissionOnFunction(string functionKey, Policy policy)
        {
            var result = false;
            if (Functions != null && Functions.Any(x => x.FunctionKey == functionKey))
            {
                switch (policy)
                {
                    case Policy.Read:
                        result = Functions.Any(x => x.FunctionKey == functionKey && (x.Policy == Policy.Read || x.Policy == Policy.Host));
                        break;

                    case Policy.Host:
                        result = Functions.Any(x => x.FunctionKey == functionKey && x.Policy == Policy.Host);
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
            session.Clear();
        }
    }
}