namespace GO.Admin.Website.Controllers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using GO.Admin.Website.Constants;
    using GO.Admin.Website.Criteria.Function;
    using GO.Admin.Website.Filters;
    using GO.Admin.Website.Services;
    using HRM.UserControl.Client.Core.Enumerables;
    using HRM.UserControl.Client.Core.Models;
    using HRM.UserControl.Client.Core.Services;
    using HRM.UserControl.Client.Core.Session;
    using Microsoft.AspNetCore.Mvc;

    [UCPermission(FunctionKey = FunctionKey.FunctionManagement, Policy = Policy.Read)]
    public class FunctionController : BaseController
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
            bool isHost = userControlSession.HasPermissionOnFunction(FunctionKey.FunctionManagement, Policy.Host);
            return View(isHost);
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
        [UCPermission(FunctionKey = FunctionKey.FunctionManagement, Policy = Policy.Host)]
        public async Task<IActionResult> Create([FromBody] CreateFunctionCommand command)
        {
            command.CreatedBy = userControlSession.UserId;
            command.FunctionKey = command.FunctionName.Trim().Replace(" ", "");
            var commandResult = await baseService.ExecuteNonQueryAsync(command);
            return Json(commandResult);
        }

        [HttpPost]
        [UCPermission(FunctionKey = FunctionKey.FunctionManagement, Policy = Policy.Host)]
        public async Task<IActionResult> Update([FromBody] EditFunctionCommand command)
        {
            command.ModifiedBy = userControlSession.UserId;
            var commandResult = await baseService.ExecuteNonQueryAsync(command);
            return Json(commandResult);
        }

        [HttpPost]
        [UCPermission(FunctionKey = FunctionKey.FunctionManagement, Policy = Policy.Host)]
        public async Task<IActionResult> UpdateStatus([FromBody] UpdateFunctionStatusCommand command)
        {
            command.ModifiedBy = userControlSession.UserId;
            CommandResult commandResult = await baseService.ExecuteNonQueryAsync(command);
            return Json(commandResult);
        }
    }
}