namespace HRM.UserControl.Admin.Website.Controllers
{
    using System.Threading.Tasks;
    using HRM.UserControl.Admin.Website.Criteria.Function;
    using HRM.UserControl.Admin.Website.Services;
    using Microsoft.AspNetCore.Mvc;
    using HRM.UserControl.Client.Core.Models;
    using HRM.UserControl.Client.Core.Services;
    using HRM.UserControl.Client.Core.Session;
    using System.Collections.Generic;

    public class FunctionController : Controller
    {
        private readonly IBaseService baseService;
        private readonly IFunctionService functionService;
        private readonly IUserControlSession userControlSession;


        public FunctionController(
            IBaseService baseService,
            IFunctionService functionService,
            IUserControlSession userControlSession)
        {
            this.baseService = baseService;
            this.functionService = functionService;
            this.userControlSession = userControlSession;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> GetBySiteId(short id)
        {
            IList<Function> functions = await functionService.GetHierarchyBySiteIdAsync(id, null);
            return Json(functions);
        }

        public async Task<IActionResult> GetParentBySiteId(short id)
        {
            IList<Function> functions = await functionService.GetBySiteIdAsync(id, null);
            return Json(functions);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateFunctionCommand command)
        {
            command.CreatedBy = userControlSession.UserId;
            command.FunctionKey = command.FunctionName.Trim().Replace(" ", "");
            var commandResult = await baseService.ExecuteNonQueryAsync(command);
            return Json(commandResult);
        }

        [HttpPost]
        public async Task<IActionResult> Update([FromBody] EditFunctionCommand command)
        {
            command.ModifiedBy = userControlSession.UserId;
            var commandResult = await baseService.ExecuteNonQueryAsync(command);
            return Json(commandResult);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStatus([FromBody] UpdateFunctionStatusCommand command)
        {
            command.ModifiedBy = userControlSession.UserId;
            CommandResult commandResult = await baseService.ExecuteNonQueryAsync(command);
            return Json(commandResult);
        }
    }
}
