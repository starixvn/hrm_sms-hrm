namespace HRM.UserControl.WebAPI.Controllers
{
    using System.Security.Claims;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    public class BaseController : Controller
    {
        private readonly IHttpContextAccessor httpContextAccessor;

        public BaseController(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
            GetIdentityFromClaims();
        }

        public int UserId { get; private set; }

        public short SiteId { get; private set; }

        private void GetIdentityFromClaims()
        {
            if (httpContextAccessor.HttpContext.User is ClaimsPrincipal identity)
            {
                UserId = int.Parse(identity.FindFirst("UserId").Value);
                SiteId = short.Parse(identity.FindFirst("SiteId").Value);
            }
        }
    }
}