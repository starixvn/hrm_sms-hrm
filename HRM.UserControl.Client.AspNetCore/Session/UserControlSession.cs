namespace HRM.UserControl.Client.AspNetCore.Session
{
    using System.Collections.Generic;
    using System.Linq;
    using HRM.UserControl.Client.Core.Constants;
    using HRM.UserControl.Client.Core.Enumerables;
    using HRM.UserControl.Client.Core.Models;
    using HRM.UserControl.Client.Core.Models.Auth;
    using HRM.UserControl.Client.Core.Session;
    using Microsoft.AspNetCore.Http;
    using Newtonsoft.Json;

    public class UserControlSession : IUserControlSession
    {
        private readonly IHttpContextAccessor accessor;

        public UserControlSession(IHttpContextAccessor accessor)
        {
            this.accessor = accessor;
        }

        public int UserId
        {
            get => accessor.HttpContext.Session?.GetInt32(SessionKey.SessionUserId) ?? 0;
            set => accessor.HttpContext.Session.SetInt32(SessionKey.SessionUserId, value);
        }

        public string Nickname
        {
            get => accessor.HttpContext.Session?.GetString(SessionKey.SessionNickname) ?? string.Empty;
            set => accessor.HttpContext.Session.SetString(SessionKey.SessionNickname, value);
        }

        public short SiteId
        {
            get => accessor.HttpContext.Session?.Get<short>(SessionKey.SessionSiteId) ?? 0;
            set => accessor.HttpContext.Session.Set<short>(SessionKey.SessionSiteId, value);
        }

        public JwtToken JwtToken
        {
            get => accessor.HttpContext.Session?.Get<JwtToken>(SessionKey.SessionJwtToken);
            set => accessor.HttpContext.Session.Set(SessionKey.SessionJwtToken, value);
        }

        public IList<FunctionPermission> Functions
        {
            get => accessor.HttpContext.Session.Get<List<FunctionPermission>>(SessionKey.SessionFunctions);
            set => accessor.HttpContext.Session.Set(SessionKey.SessionFunctions, value);
        }

        public IList<MenuItem> MenuItems
        {
            get => accessor.HttpContext.Session.Get<List<MenuItem>>(SessionKey.SessionMenuItems);
            set => accessor.HttpContext.Session.Set(SessionKey.SessionMenuItems, value);
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
            accessor.HttpContext.Session.Clear();
        }
    }

    internal static class SessionExtensions
    {
        public static void Set<T>(this ISession session, string key, T value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }

        public static T Get<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default : JsonConvert.DeserializeObject<T>(value);
        }
    }
}