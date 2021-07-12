using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace HRM.UserControl.WebAPI.Controllers
{
    public class BaseController : Controller
    {
        public BaseController()
        {
            GetIdentityFromClaims();
        }

        public int UserId { get; private set; }

        public int SiteId { get; private set; }

        private void GetIdentityFromClaims()
        {
            if (HttpContext.User.Identity is ClaimsIdentity identity)
            {
                UserId = int.Parse(identity.FindFirst("UserId").Value);
                SiteId = int.Parse(identity.FindFirst("SiteId").Value);
            }
        }
    }
}