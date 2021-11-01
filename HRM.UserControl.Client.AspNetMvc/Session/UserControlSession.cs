namespace HRM.UserControl.Client.AspNetMvc.Session
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.SessionState;
    using HRM.UserControl.Client.Core.Constants;
    using HRM.UserControl.Client.Core.Enumerables;
    using HRM.UserControl.Client.Core.Models;
    using HRM.UserControl.Client.Core.Session;
    using Newtonsoft.Json;

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

        public IList<FunctionPermission> Functions
        {
            get
            {
                if (Session[SessionKey.SessionFunctions] != null)
                {
                    return JsonConvert.DeserializeObject<IList<FunctionPermission>>(Session[SessionKey.SessionFunctions]?.ToString());
                }

                return new List<FunctionPermission>();
            }
            set => Session.Add(SessionKey.SessionFunctions, JsonConvert.SerializeObject(value));
        }

        public IList<MenuItem> MenuItems
        {
            get
            {
                if (Session[SessionKey.SessionMenuItems] != null)
                {
                    return JsonConvert.DeserializeObject<IList<MenuItem>>(Session[SessionKey.SessionMenuItems]?.ToString());
                }

                return new List<MenuItem>();
            }
            set => Session.Add(SessionKey.SessionMenuItems, JsonConvert.SerializeObject(value));
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
            Session.Abandon();
            Session.RemoveAll();
        }
    }
}