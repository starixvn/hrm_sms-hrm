namespace HRM.UserControl.Admin.Website.Controllers
{
    using System.Threading.Tasks;
    using HRM.UserControl.Admin.Website.Criteria.User;
    using HRM.UserControl.Admin.Website.Models;
    using HRM.UserControl.Admin.Website.Models.User;
    using HRM.UserControl.Admin.Website.Services;
    using HRM.UserControl.Client.Core.Models.User;
    using HRM.UserControl.Client.Core.Services;
    using Microsoft.AspNetCore.Mvc;

    public class UserController : Controller
    {
        private readonly IBaseService baseService;
        private readonly ICryptoService cryptoService;
        private readonly IUserService userService;

        public UserController(
            IBaseService baseService,
            ICryptoService cryptoService,
            IUserService userService)
        {
            this.baseService = baseService;
            this.cryptoService = cryptoService;
            this.userService = userService;
        }

        public IActionResult Index()
        {
            var token = cryptoService.GenerateRijndaelKeyIV();
            return View("Index", token);
        }

        [HttpPost]
        public async Task<IActionResult> GetAll([FromBody] GetAllUserCriteria criteria)
        {
            var users = await baseService.GetDataAsync<Models.User.User>(criteria);
            return Json(new { totalItems = criteria.TotalRows, data = users });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateUserCommand command)
        {
            string[] keyiv = command.Token.Split(":");
            var rijndaelKey = keyiv[0];
            var rijndaelIV = keyiv[1];
            command.Username = cryptoService.Decrypt(command.Username, rijndaelKey, rijndaelIV);
            command.Password = cryptoService.Decrypt(command.Password, rijndaelKey, rijndaelIV);
            command.Password = BCrypt.Net.BCrypt.HashPassword(command.Password);

            command.CreatedBy = 4;
            Website.Models.CommandResult commandResult = await baseService.ExecuteNonQueryAsync(command);
            return Json(commandResult);
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword([FromBody] UpdatePasswordParam param)
        {
            Client.Core.Models.CommandResult commandResult = await userService.ResetPasswordAsync(param);
            return Json(commandResult);
        }
    }
}