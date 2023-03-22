namespace GO.WebAPI.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using GO.Core.Extensions.XML;
    using GO.Core.Models;
    using GO.Core.Services;
    using GO.WebAPI.Criteria.UnitHead;
    using GO.WebAPI.Models.UnitHead;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    [Authorize]
    public class UnitHeadController : BaseController
    {
        private readonly IBaseService baseService;

        public UnitHeadController(
            IHttpContextAccessor httpContextAccessor,
            IBaseService baseService) : base(httpContextAccessor)
        {
            this.baseService = baseService;
        }

        [HttpGet]
        [Route("api/UnitHead/GetInfo")]
        public async Task<IActionResult> GetInfo()
        {
            var criteria = new GetUnitHeadInfoCriteria();
            var infos = await baseService.GetDataAsync<UnitHeadInfo>(criteria);

            return Json(infos);
        }

        [HttpPost]
        [Route("api/UnitHead/UpdateInfo")]
        public async Task<IActionResult> UpdateInfo([FromBody] IList<UnitHeadInfo> infos)
        {
            var command = new UpdateUnitHeadInfoCommand
            {
                SiteId = SiteId,
                ModifiedBy = UserId,
                Value = infos != null && infos.Any() ? infos.ToXML().ToString() : string.Empty
            };
            CommandResult commandResult = await baseService.ExecuteNonQueryAsync(command);

            return Json(commandResult);
        }

        [HttpGet]
        [Route("api/UnitHead/GetSchedule")]
        public async Task<IActionResult> GetSchedule()
        {
            var criteria = new GetUnitHeadScheduleCriteria
            {
                FromDate = DateTime.Today,
                ToDate = DateTime.Today.AddMonths(1)
            };
            var schedules = await baseService.GetDataAsync<UnitHeadSchedule>(criteria);

            return Json(schedules);
        }

        [HttpPost]
        [Route("api/UnitHead/UpdateSchedule")]
        public async Task<IActionResult> UpdateSchedule([FromBody] IList<UnitHeadSchedule> schedules)
        {
            var command = new UpdateUnitHeadScheduleCommand
            {
                SiteId = SiteId,
                ModifiedBy = UserId,
                Value = schedules != null && schedules.Any() ? schedules.ToXML().ToString() : string.Empty
            };
            CommandResult commandResult = await baseService.ExecuteNonQueryAsync(command);

            return Json(commandResult);
        }
    }
}