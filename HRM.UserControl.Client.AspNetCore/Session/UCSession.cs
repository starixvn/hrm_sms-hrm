namespace HRM.UserControl.Client.AspNetCore.Session
{
    using System.Collections.Generic;
    using HRM.UserControl.Client.Core.Helpers.Constants;
    using HRM.UserControl.Client.Core.Models.Role;
    using HRM.UserControl.Client.Core.Session;
    using Microsoft.AspNetCore.Http;
    using Newtonsoft.Json;

    public class UCSession : IUClSession
    {
        private readonly IHttpContextAccessor accessor;

        public UCSession(IHttpContextAccessor accessor)
        {
            this.accessor = accessor;
        }

        public int UserId
        {
            get => accessor.HttpContext.Session?.GetInt32(SessionKey.SessionUserId) ?? 0;
            set => accessor.HttpContext.Session.SetInt32(SessionKey.SessionUserId, value);
        }

        public int OnCompanyId
        {
            get => accessor.HttpContext.Session?.GetInt32(SessionKey.SessionOnCompanyId) ?? 0;
            set => accessor.HttpContext.Session.SetInt32(SessionKey.SessionOnCompanyId, value);
        }

        public string JwtToken
        {
            get => accessor.HttpContext.Session?.GetString(SessionKey.SessionJwtToken) ?? string.Empty;
            set => accessor.HttpContext.Session.SetString(SessionKey.SessionJwtToken, value);
        }

        public IList<CompanyRoleFunction> Roles
        {
            get => accessor.HttpContext.Session.Get<List<CompanyRoleFunction>>(SessionKey.SessionRoles);
            set => accessor.HttpContext.Session.Set(SessionKey.SessionRoles, value);
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