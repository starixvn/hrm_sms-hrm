namespace HRM.UserControl.Admin.Website.Services
{
    using System.Collections.Generic;
    using HRM.UserControl.Admin.Website.Constants;
    using HRM.UserControl.Admin.Website.Extensions;
    using HRM.UserControl.Admin.Website.Models;
    using HRM.UserControl.Admin.Website.Models.Session;
    using Microsoft.AspNetCore.Http;

    public interface ISessionService
    {
        UserSession User { get; set; }

        IEnumerable<Company> Companies { get; set; }

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

        public IEnumerable<Company> Companies
        {
            get => session.Get<List<Company>>(SessionKey.InternalCompanies);
            set => session.Set(SessionKey.InternalCompanies, value);
        }

        public void ClearAll()
        {
            session.Clear();
        }
    }
}