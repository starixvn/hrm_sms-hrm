namespace GO.Website.Controllers.Manpower
{
    using System.Linq;
    using System.Threading.Tasks;
    using Filters;
    using GO.Core.Extensions.XML;
    using GO.Core.Services;
    using Constants;
    using GO.Website.Criteria.Manpower.Period;
    using GO.Website.Models.Manpower.Period;
    using HRM.UserControl.Client.Core.Enumerables;
    using HRM.UserControl.Client.Core.Session;
    using Microsoft.AspNetCore.Mvc;

    [Route("Manpower/Period/[action]/{id?}")]
    [GOPermission(FunctionKey = FunctionKey.ManpowerForecast_PeriodManagement, Policy = Policy.Read)]
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
            var isHost = userControlSession.HasPermissionOnFunction(FunctionKey.ManpowerForecast_PeriodManagement, Policy.Host);
            return View("~/Views/Manpower/Period/Index.cshtml", isHost);
        }

        public IActionResult Detail(int id)
        {
            var isHost = userControlSession.HasPermissionOnFunction(FunctionKey.ManpowerForecast_PeriodManagement, Policy.Host);
            ViewBag.periodId = id;
            return View("~/Views/Manpower/Period/Detail.cshtml", isHost);
        }

        [HttpPost]
        public async Task<IActionResult> GetPeriods([FromBody] GetPeriodCriteria criteria)
        {
            var(periods, processSettings) = await baseService.FetchMultipleAsync<ManpowerPeriod, MapnowerPeriodProcessSetting>(criteria);
            foreach (var period in periods)
            {
                period.Settings = processSettings.Where(x => x.PeriodId == period.PeriodId).ToList();
            }

            return Json(new { totalItems = criteria.TotalRows, data = periods });
        }

        [HttpPost]
        public async Task<IActionResult> GetById([FromBody] GetPeriodByIdCriteria criteria)
        {
            var(periods, processSettings) = await baseService.FetchMultipleAsync<ManpowerPeriod, MapnowerPeriodProcessSetting>(criteria);
            foreach (var period in periods)
            {
                period.Settings = processSettings.Where(x => x.PeriodId == period.PeriodId).ToList();
            }

            return Json(periods);
        }

        [HttpPost]
        [GOPermission(FunctionKey = FunctionKey.ManpowerForecast_PeriodManagement, Policy = Policy.Host)]
        public async Task<IActionResult> Create([FromBody] CreatePeriodCommand command)
        {
            command.SettingXml = command.Settings != null && command.Settings.Any() ? command.Settings.ToXML().ToString() : string.Empty;
            command.IsActive = true;
            command.CreatedBy = userControlSession.UserId;
            var commandResult = await baseService.ExecuteNonQueryAsync(command);

            return Json(commandResult);
        }

        [HttpPost]
        [GOPermission(FunctionKey = FunctionKey.ManpowerForecast_PeriodManagement, Policy = Policy.Host)]
        public async Task<IActionResult> Update([FromBody] UpdatePeriodCommand command)
        {
            command.SettingXml = command.Settings != null && command.Settings.Any() ? command.Settings.ToXML().ToString() : string.Empty;
            command.ModifiedBy = userControlSession.UserId;
            var commandResult = await baseService.ExecuteNonQueryAsync(command);

            return Json(commandResult);
        }

        [HttpPost]
        [GOPermission(FunctionKey = FunctionKey.ManpowerForecast_PeriodManagement, Policy = Policy.Host)]
        public async Task<IActionResult> UpdateStatus([FromBody] UpdateStatusPeriodCommand command)
        {
            command.ModifiedBy = userControlSession.UserId;
            var commandResult = await baseService.ExecuteNonQueryAsync(command);

            return Json(commandResult);
        }

        [HttpPost]
        [GOPermission(FunctionKey = FunctionKey.ManpowerForecast_PeriodManagement, Policy = Policy.Host)]
        public async Task<IActionResult> Delete([FromBody] DeletePeriodCommand command)
        {
            command.ModifiedBy = userControlSession.UserId;
            var commandResult = await baseService.ExecuteNonQueryAsync(command);

            return Json(commandResult);
        }
    }
}