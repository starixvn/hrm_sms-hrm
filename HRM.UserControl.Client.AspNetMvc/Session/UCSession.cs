namespace HRM.UserControl.Client.AspNetMvc.Session
{
    using System.Collections.Generic;
    using System.Web;
    using System.Web.SessionState;
    using HRM.UserControl.Client.Core.Helpers.Constants;
    using HRM.UserControl.Client.Core.Models.Role;
    using HRM.UserControl.Client.Core.Session;
    using Newtonsoft.Json;

    public class UCSession : IUCSession
    {
        private static HttpSessionState Session => HttpContext.Current.Session;

        public int UserId

        {
            get => int.Parse(Session[SessionKey.SessionUserId]?.ToString() ?? "0");
            set => Session.Add(SessionKey.SessionUserId, value);
        }

        public int OnCompanyId
        {
            get => int.Parse(Session[SessionKey.SessionOnCompanyId]?.ToString() ?? "0");
            set => Session.Add(SessionKey.SessionOnCompanyId, value);
        }

        public string JwtToken
        {
            get => Session?[SessionKey.SessionJwtToken]?.ToString() ?? string.Empty;
            set => Session.Add(SessionKey.SessionJwtToken, value);
        }

        public IList<CompanyRoleFunction> Roles
        {
            get => JsonConvert.DeserializeObject<IList<CompanyRoleFunction>>(Session[SessionKey.SessionJwtToken]?.ToString());
            set => Session.Add(SessionKey.SessionJwtToken, JsonConvert.SerializeObject(value));
        }

        public void ClearAll()
        {
            Session.Abandon();
            Session.RemoveAll();
        }
    }
}