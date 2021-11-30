namespace HRM.UserControl.Admin.Website.Controllers
{
    using System.Threading.Tasks;
    using HRM.UserControl.Admin.Website.Criteria.Function;
    using HRM.UserControl.Admin.Website.Services;
    using Microsoft.AspNetCore.Mvc;
    using HRM.UserControl.Client.Core.Models;
    using HRM.UserControl.Client.Core.Services;
    using HRM.UserControl.Admin.Website.Criteria.Site;
    using HRM.UserControl.Admin.Website.Models.Site;

    public class FunctionController : Controller
    {
        private readonly IBaseService baseService;
        private readonly IFunctionService functionService;

        public FunctionController(IBaseService baseService, IFunctionService functionService)
        {
            this.baseService = baseService;
            this.functionService = functionService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> GetBySiteId(short id)
        {
            var functions = await functionService.GetBySiteIdAsync(id, null);
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
