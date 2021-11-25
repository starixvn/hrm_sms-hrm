namespace HRM.UserControl.Admin.Website.Controllers
{
    using System.Threading.Tasks;
    using System.Linq;
    using HRM.UserControl.Admin.Website.Criteria.Function;
    using HRM.UserControl.Admin.Website.Criteria.Site;
    using HRM.UserControl.Admin.Website.Models.Function;
    using HRM.UserControl.Admin.Website.Models.Site;
    using HRM.UserControl.Admin.Website.Services;
    using Microsoft.AspNetCore.Mvc;
    using System.Collections.Generic;
    using HRM.UserControl.Admin.Website.Models;

    public class FunctionController : Controller
    {
        private readonly IBaseService baseService;

        public FunctionController(IBaseService baseService)
        {
            this.baseService = baseService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> GetAllSites()
        {
            var criteria = new GetAllSiteCriteria { IsActive = true };
            var sites = await baseService.GetDataAsync<Site>(criteria);
            return Json(sites);
        }

        public async Task<IActionResult> GetById(short id)
        {
            var criteria = new GetFunctionBySiteIdCriteria { SiteId = id };
            var functions = await baseService.GetDataAsync<Function>(criteria);
            return Json(functions);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateFunctionCommand command)
        {
            command.CreatedBy = 4;
            command.FunctionKey = command.FunctionName.Trim().Replace(" ", "");
            var commandResult = await baseService.ExecuteNonQueryAsync(command);
            return Json(commandResult);
        }

        [HttpPost]
        public async Task<IActionResult> Update([FromBody] EditFunctionCommand command)
        {
            command.ModifiedBy = 4;
            var commandResult = await baseService.ExecuteNonQueryAsync(command);
            return Json(commandResult);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStatus([FromBody] UpdateFunctionStatusCommand command)
        {
            command.ModifiedBy = 4;
            CommandResult commandResult = await baseService.ExecuteNonQueryAsync(command);
            return Json(commandResult);
        }
    }
}
