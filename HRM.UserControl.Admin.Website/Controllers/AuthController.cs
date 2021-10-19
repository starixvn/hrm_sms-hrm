namespace HRM.UserControl.Admin.Website.Controllers
{
    using System.Collections.Generic;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using HRM.UserControl.Admin.Website.Criteria.Auth;
    using HRM.UserControl.Admin.Website.Models;
    using HRM.UserControl.Admin.Website.Models.Auth;
    using HRM.UserControl.Admin.Website.Models.Session;
    using HRM.UserControl.Admin.Website.Resources;
    using HRM.UserControl.Admin.Website.Services;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.AspNetCore.Mvc;

    public class AuthController : Controller
    {
        private readonly ISessionService sessionService;
        private readonly ICryptoService cryptoService;
        private readonly IBaseService baseService;

        public AuthController(
            ISessionService sessionService,
            ICryptoService cryptoService,
            IBaseService baseService)
        {
            this.sessionService = sessionService;
            this.cryptoService = cryptoService;
            this.baseService = baseService;
        }

        public IActionResult Login()
        {
            var token = cryptoService.GenerateRijndaelKeyIV();
            return View("Login", token);
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginParam model)
        {
            string[] keyiv = model.Token.Split(":");
            var rijndaelKey = keyiv[0];
            var rijndaelIV = keyiv[1];
            model.Username = cryptoService.Decrypt(model.Username, rijndaelKey, rijndaelIV);
            model.Password = cryptoService.Decrypt(model.Password, rijndaelKey, rijndaelIV);

            var criteria = new GetUserByUsernameCriteria
            {
                Username = model.Username
            };
            var user = await baseService.GetDataEntityAsync<UserSession>(criteria);
            if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.Password))
            {
                return Json(new CommandResult { Code = 1, Message = ViewResources.WrongUsernameOrPassword });
            }

            if (!user.IsActive)
            {
                return Json(new CommandResult { Code = 1, Message = ViewResources.DisabledUser });
            }

            if (user.IsDeleted)
            {
                return Json(new CommandResult { Code = 1, Message = ViewResources.DeletedUser });
            }

            await DoLoginAsync(user);
            return Json(new CommandResult { Code = 0, Message = string.Empty });
        }

        protected internal virtual async Task DoLoginAsync(UserSession user)
        {
            sessionService.User = user;

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString())
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));
        }
    }
}