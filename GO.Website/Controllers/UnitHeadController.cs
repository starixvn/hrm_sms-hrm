namespace GO.Website.Controllers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using GO.Website.Criteria.UnitHead;
    using GO.Website.Models.UnitHead;
    using GO.Website.Services;
    using Microsoft.AspNetCore.Mvc;

    public class UnitHeadController : Controller
    {
        private readonly IUnitHeadService unitHeadService;

        public UnitHeadController(IUnitHeadService unitHeadService)
        {
            this.unitHeadService = unitHeadService;
        }

        public IActionResult Schedule()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GetWeeklySchedule([FromBody] GetUnitHeadWeeklyScheduleCriteria criteria)
        {
            IEnumerable<UnitHeadWeeklySchedule> schedules = await unitHeadService.GetWeeklyScheduleAsync(criteria);

            return Json(schedules);
        }
    }
}