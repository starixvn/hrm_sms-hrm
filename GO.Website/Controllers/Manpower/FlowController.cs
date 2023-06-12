namespace GO.Website.Controllers.Manpower
{
    using System.Linq;
    using System.Threading.Tasks;
    using GO.Admin.Website.Filters;
    using GO.Core.Extensions.XML;
    using GO.Core.Models;
    using GO.Core.Services;
    using GO.Website.Constants;
    using GO.Website.Criteria.Manpower;
    using GO.Website.Models;
    using GO.Website.Models.Manpower;
    using HRM.UserControl.Client.Core.Enumerables;
    using HRM.UserControl.Client.Core.Session;
    using Microsoft.AspNetCore.Mvc;

    [Route("Manpower/Flow/[action]")]
    [GOPermission(FunctionKey = FunctionKey.Manpower_FlowManagement, Policy = Policy.Read)]
    public class FlowController : BaseController
    {
        private readonly IUserControlSession userControlSession;
        private readonly IBaseService baseService;

        public FlowController(
            IUserControlSession userControlSession,
            IBaseService baseService)
        {
            this.userControlSession = userControlSession;
            this.baseService = baseService;
        }

        public IActionResult Index()
        {
            bool isHost = userControlSession.HasPermissionOnFunction(FunctionKey.Manpower_FlowManagement, Policy.Host);
            return View("~/Views/Manpower/Flow/Index.cshtml", isHost);
        }

        public async Task<IActionResult> GetUnits()
        {
            var criteria = new GetUnitCriteria();
            var units = await baseService.GetDataAsync<Unit>(criteria);
            return Json(units);
        }

        public async Task<IActionResult> GetFlows()
        {
            var criteria = new GetFlowCriteria();
            var flows = await baseService.GetDataAsync<ManpowerFlow>(criteria);
            return Json(flows);
        }

        public async Task<IActionResult> GetByUnit(GetFlowByUnitCriteria criteria)
        {
            var flows = await baseService.GetDataAsync<ManpowerFlow>(criteria);
            return Json(flows);
        }

        [HttpPost]
        [GOPermission(FunctionKey = FunctionKey.Manpower_FlowManagement, Policy = Policy.Host)]
        public async Task<IActionResult> Update([FromBody] UpdateFlowParam param)
        {
            var command = new UpdateFlowCommand
            {
                UnitName = param.UnitName,
                FlowXml = param.Flows != null && param.Flows.Any() ? param.Flows.ToXML().ToString() : string.Empty,
                ModifiedBy = userControlSession.UserId
            };
            CommandResult commandResult = await baseService.ExecuteNonQueryAsync(command);
            return Json(commandResult);
        }

        [HttpPost]
        [GOPermission(FunctionKey = FunctionKey.Manpower_FlowManagement, Policy = Policy.Host)]
        public async Task<IActionResult> Delete([FromBody] DeleteFlowCommand command)
        {
            command.ModifiedBy = userControlSession.UserId;
            CommandResult commandResult = await baseService.ExecuteNonQueryAsync(command);
            return Json(commandResult);
        }
    }
}