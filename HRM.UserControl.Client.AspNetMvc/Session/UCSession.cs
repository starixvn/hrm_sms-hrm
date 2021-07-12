namespace HRM.UserControl.Client.AspNetMvc.Session
{
    using System.Collections.Generic;
    using System.Web;
    using HRM.UserControl.Client.Core.Helpers.Constants;
    using HRM.UserControl.Client.Core.Models.Role;
    using HRM.UserControl.Client.Core.Session;
    using Newtonsoft.Json;

    public class UCSession : IUClSession
    {
        public int UserId
        {
            get => int.Parse(HttpContext.Current.Session[SessionKey.SessionUserId]?.ToString() ?? "0");
            set => HttpContext.Current.Session.Add(SessionKey.SessionUserId, value);
        }

        public int OnCompanyId
        {
            get => int.Parse(HttpContext.Current.Session[SessionKey.SessionOnCompanyId]?.ToString() ?? "0");
            set => HttpContext.Current.Session.Add(SessionKey.SessionOnCompanyId, value);
        }

        public string JwtToken
        {
            get => HttpContext.Current.Session[SessionKey.SessionJwtToken]?.ToString() ?? string.Empty;
            set => HttpContext.Current.Session.Add(SessionKey.SessionJwtToken, value);
        }

        public IList<CompanyRoleFunction> Roles
        {
            get => JsonConvert.DeserializeObject<IList<CompanyRoleFunction>>(HttpContext.Current.Session[SessionKey.SessionJwtToken]?.ToString());
            set => HttpContext.Current.Session.Add(SessionKey.SessionJwtToken, JsonConvert.SerializeObject(value));
        }

        public void ClearAll()
        {
            HttpContext.Current.Session.Clear();
        }
    }
}