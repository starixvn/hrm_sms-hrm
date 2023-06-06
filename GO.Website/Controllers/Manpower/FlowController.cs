namespace GO.Website.Controllers.Manpower
{
    using System.Linq;
    using System.Threading.Tasks;
    using GO.Core.Services;
    using GO.Core.Extensions.XML;
    using GO.Website.Constants;
    using GO.Website.Criteria.Manpower;
    using GO.Website.Models;
    using GO.Website.Models.Manpower;
    using HRM.UserControl.Client.Core.Enumerables;
    using HRM.UserControl.Client.Core.Session;
    using Microsoft.AspNetCore.Mvc;
    using GO.Core.Models;

    [Route("Manpower/Flow/[action]")]
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
            bool isHost = userControlSession.HasPermissionOnFunction(FunctionKey.Manpower_EmployeeManagement, Policy.Host);
            return View("~/Views/Manpower/Flow/Index.cshtml", isHost);
        }

        public async Task<IActionResult> GetUnits()
        {
            var criteria = new GetUnitCriteria();
            var units = await baseService.GetDataAsync<Unit>(criteria);
            return Json(units);
        }

        public async Task<IActionResult> GetFlowByUnit(GetFlowByUnitCriteria criteria)
        {
            var flows = await baseService.GetDataAsync<ManpowerFlow>(criteria);
            return Json(flows);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateFlow([FromBody] UpdateFlowParam param)
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
    }
}