namespace GO.Website.Controllers.Manpower
{
    using GO.Admin.Website.Filters;
    using GO.Core.Extensions.XML;
    using GO.Core.Models;
    using GO.Core.Services;
    using GO.Website.Constants;
    using GO.Website.Criteria.Manpower.Period;
    using GO.Website.Models.Manpower.Period;
    using HRM.UserControl.Client.Core.Enumerables;
    using HRM.UserControl.Client.Core.Session;
    using Microsoft.AspNetCore.Mvc;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    [Route("Manpower/Period/[action]")]
    [GOPermission(FunctionKey = FunctionKey.Manpower_PeriodManagement, Policy = Policy.Read)]
    public class PeriodController : BaseController
    {
        private readonly IUserControlSession userControlSession;
        private readonly IBaseService baseService;

        public PeriodController(
            IUserControlSession userControlSession,
            IBaseService baseService)
        {
            this.userControlSession = userControlSession;
            this.baseService = baseService;
        }
        public IActionResult Index()
        {
            bool isHost = userControlSession.HasPermissionOnFunction(FunctionKey.Manpower_PeriodManagement, Policy.Host);
            return View("~/Views/Manpower/Period/Index.cshtml", isHost);
        }

        [HttpPost]
        public async Task<IActionResult> GetPeriodInfos([FromBody] GetPeriodCriteria criteria)
        {
            var tuple = await baseService.FetchMultipleAsync<PeriodInfo, PeriodProcessSettingInfo>(criteria);
            IList<PeriodInfo> periods = tuple.Item1.ToList();
            foreach (var period in periods)
            {
                period.Settings = tuple.Item2.Where(x => x.PeriodId == period.PeriodId).ToList();
            }
            return Json(new { totalItems = criteria.TotalRows, data = periods });
        }

        [HttpPost]
        [GOPermission(FunctionKey = FunctionKey.Manpower_PeriodManagement, Policy = Policy.Host)]
        public async Task<IActionResult> Create([FromBody] InsertPeriodCommand command)
        {
            command.CreatedBy = userControlSession.UserId;
            if (command != null)
            {
                command.SettingXml = command.Settings.ToXML().ToString();
                command.IsActive = true;
                command.CreatedBy = userControlSession.UserId;
            }
            CommandResult commandResult = await baseService.ExecuteNonQueryAsync(command);
            return Json(commandResult);
        }

        [HttpPost]
        [GOPermission(FunctionKey = FunctionKey.Manpower_PeriodManagement, Policy = Policy.Host)]
        public async Task<IActionResult> UpdateIsActive([FromBody] UpdateStatusPeriodCommand command)
        {
            command.ModifiedBy = userControlSession.UserId;
            command.SiteId = userControlSession.SiteId;
            CommandResult commandResult = await baseService.ExecuteNonQueryAsync(command);
            return Json(commandResult);
        }


        [HttpPost]
        [GOPermission(FunctionKey = FunctionKey.Manpower_PeriodManagement, Policy = Policy.Host)]
        public async Task<IActionResult> Delete([FromBody] DeletePeriodCommand command)
        {
            command.ModifiedBy = userControlSession.UserId;
            command.SiteId = userControlSession.SiteId;
            CommandResult commandResult = await baseService.ExecuteNonQueryAsync(command);
            return Json(commandResult);
        }
    }
}
