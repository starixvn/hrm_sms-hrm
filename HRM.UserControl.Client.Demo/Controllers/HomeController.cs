namespace HRM.UserControl.Client.Demo.Controllers
{
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using HRM.UserControl.Client.AspNetMvc.Session;
    using HRM.UserControl.Client.Core.Models.Auth;
    using HRM.UserControl.Client.Core.Services;
    using HRM.UserControl.Client.Core.Session;

    public class HomeController : Controller
    {
        private readonly ICryptoService cryptoService;
        private readonly IUserService userService;
        private readonly ISecurityService securityService;
        private readonly IUserControlSession userControlSession;

        public HomeController() : this(null, null, null, null)
        {
        }

        public HomeController(
            ICryptoService cryptoService,
            IUserService userService,
            IUserControlSession userControlSession,
            ISecurityService securityService)
        {
            this.cryptoService = cryptoService ?? new CryptoService();
            this.userService = userService ?? new UserService();
            this.userControlSession = userControlSession ?? new UserControlSession();
            this.securityService = securityService ?? new SecurityService();
        }

        public async Task<ActionResult> Index()
        {
            var token = cryptoService.GenerateRijndaelKeyIV();
            string[] keyiv = token.Split(':');
            var rijndaelKey = keyiv[0];
            var rijndaelIV = keyiv[1];
            var loginParam = new LoginParam
            {
                Username = cryptoService.Encrypt("oliver.dang", rijndaelKey, rijndaelIV),
                Password = cryptoService.Encrypt("1234aa", rijndaelKey, rijndaelIV),
                Token = token,
                IP = Request.UserHostAddress,
                Platform = Request.Headers["User-Agent"],
                BrowserName = Request.Headers["User-Agent"]
            };
            var loginResult = await securityService.LoginAsync(loginParam);

            if (loginResult.IsSuccessfull)
            {
                userControlSession.UserId = loginResult.UserId;
                userControlSession.JwtToken = loginResult.JwtToken;
                userControlSession.MenuItems = userService.GetMenuItem();
            }

            return Json(loginResult, JsonRequestBehavior.AllowGet);
        }
    }
}