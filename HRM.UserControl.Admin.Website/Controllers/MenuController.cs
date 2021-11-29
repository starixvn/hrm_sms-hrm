namespace HRM.UserControl.Admin.Website.Controllers
{
    using System.Threading.Tasks;
    using HRM.UserControl.Admin.Website.Criteria.Menu;
    using HRM.UserControl.Admin.Website.Criteria.Site;
    using HRM.UserControl.Admin.Website.Models.Menu;
    using HRM.UserControl.Admin.Website.Models.Site;
    using HRM.UserControl.Admin.Website.Services;
    using HRM.UserControl.Client.Core.Models;
    using Microsoft.AspNetCore.Mvc;


    public class MenuController : Controller
    {
        private readonly IBaseService baseService;

        public MenuController(IBaseService baseService)
        {
            this.baseService = baseService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> GetAllSites()
        {
            var criteria = new GetAllSiteCriteria();
            var sites = await baseService.GetDataAsync<Site>(criteria);
            return Json(sites);
        }

        public async Task<IActionResult> GetById(short id)
        {
            var criteria = new GetMenuBySiteId { SiteId = id };
            var menus = await baseService.GetDataAsync<Menu>(criteria);
            return Json(menus);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateMenuCommand command)
        {
            command.CreatedBy = 4;
            var commandResult = await baseService.ExecuteNonQueryAsync(command);
            return Json(commandResult);
        }

        [HttpPost]
        public async Task<IActionResult> Update([FromBody] EditMenuCommand command)
        {
            command.ModifiedBy = 4;
            var commandResult = await baseService.ExecuteNonQueryAsync(command);
            return Json(commandResult);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStatus([FromBody] UpdateMenuStatusCommand command)
        {
            command.ModifiedBy = 4;
            CommandResult commandResult = await baseService.ExecuteNonQueryAsync(command);
            return Json(commandResult);
        }
    }
}
