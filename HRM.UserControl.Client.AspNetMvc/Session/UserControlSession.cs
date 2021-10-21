namespace HRM.UserControl.Client.AspNetMvc.Session
{
    using System.Web;
    using System.Web.SessionState;
    using HRM.UserControl.Client.Core.Helpers.Constants;
    using HRM.UserControl.Client.Core.Session;

    public class UserControlSession : IUserControlSession, IRequiresSessionState
    {
        private static HttpSessionState Session => HttpContext.Current?.Session;

        public int UserId

        {
            get => int.Parse(Session[SessionKey.SessionUserId]?.ToString() ?? "0");
            set => Session.Add(SessionKey.SessionUserId, value);
        }

        public string JwtToken
        {
            get => Session?[SessionKey.SessionJwtToken]?.ToString() ?? string.Empty;
            set => Session.Add(SessionKey.SessionJwtToken, value);
        }

        public void ClearAll()
        {
            Session.Abandon();
            Session.RemoveAll();
        }
    }
}