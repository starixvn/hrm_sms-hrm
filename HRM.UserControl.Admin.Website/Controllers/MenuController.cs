namespace HRM.UserControl.Admin.Website.Controllers
{
    using System.Threading.Tasks;
    using HRM.UserControl.Admin.Website.Criteria.Menu;
    using HRM.UserControl.Admin.Website.Models.Menu;
    using HRM.UserControl.Admin.Website.Services;
    using HRM.UserControl.Client.Core.Models;
    using HRM.UserControl.Client.Core.Session;
    using Microsoft.AspNetCore.Mvc;


    public class MenuController : Controller
    {
        private readonly IBaseService baseService;
        private readonly IUserControlSession userControlSession;

        public MenuController(IBaseService baseService, IUserControlSession userControlSession)
        {
            this.baseService = baseService;
            this.userControlSession = userControlSession;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> GetBySiteId(short id)
        {
            var criteria = new GetMenuBySiteId { SiteId = id };
            var menus = await baseService.GetDataAsync<Menu>(criteria);
            return Json(menus);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateMenuCommand command)
        {
            command.CreatedBy = userControlSession.UserId;
            var commandResult = await baseService.ExecuteNonQueryAsync(command);
            return Json(commandResult);
        }

        [HttpPost]
        public async Task<IActionResult> Update([FromBody] EditMenuCommand command)
        {
            command.ModifiedBy = userControlSession.UserId;
            var commandResult = await baseService.ExecuteNonQueryAsync(command);
            return Json(commandResult);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStatus([FromBody] UpdateMenuStatusCommand command)
        {
            command.ModifiedBy = userControlSession.UserId;
            CommandResult commandResult = await baseService.ExecuteNonQueryAsync(command);
            return Json(commandResult);
        }
    }
}
