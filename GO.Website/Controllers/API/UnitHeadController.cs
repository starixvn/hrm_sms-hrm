namespace GO.Website.Controllers.API
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using GO.Core.Services;
    using GO.Website.Criteria.UnitHead;
    using GO.Website.Models.UnitHead;
    using Microsoft.AspNetCore.Mvc;

    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UnitHeadController : ControllerBase
    {
        private readonly IBaseService baseService;

        public UnitHeadController(IBaseService baseService)
        {
            this.baseService = baseService;
        }

        [HttpGet]
        [ResponseCache(Duration = 30)]
        public async Task<IEnumerable<UnitHeadInfo>> GetInfo()
        {
            var criteria = new GetUnitHeadInfoCriteria();
            var infos = await baseService.GetDataAsync<UnitHeadInfo>(criteria);

            return infos;
        }

        [HttpGet]
        [ResponseCache(Duration = 30)]
        public async Task<IEnumerable<UnitHeadDailySchedule>> GetSchedule()
        {
            var criteria = new GetUnitHeadScheduleCriteria
            {
                FromDate = DateTime.Today,
                ToDate = DateTime.Today.AddMonths(1)
            };
            var schedules = await baseService.GetDataAsync<UnitHeadDailySchedule>(criteria);

            return schedules;
        }
    }
}