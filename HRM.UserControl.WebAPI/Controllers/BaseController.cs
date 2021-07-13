using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HRM.UserControl.WebAPI.Controllers
{
    public class BaseController : Controller
    {
        private readonly IHttpContextAccessor httpContextAccessor;

        public BaseController(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
            GetIdentityFromClaims();
        }

        public int UserId { get; private set; }

        public int SiteId { get; private set; }

        private void GetIdentityFromClaims()
        {
            if (httpContextAccessor.HttpContext.User is ClaimsPrincipal identity)
            {
                UserId = int.Parse(identity.FindFirst("UserId").Value);
                SiteId = int.Parse(identity.FindFirst("SiteId").Value);
            }
        }
    }
}