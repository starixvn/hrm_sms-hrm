namespace Fanex.NexHR.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Constants;
    using Fanex.NexHR.Core.Services;
    using Fanex.NexHR.Criteria.Calendar;
    using Fanex.NexHR.Models.Report;
    using Fanex.NexHR.Models.ScheduleArrange;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.ViewModels;

    public class CalendarController : BaseController
    {
        private readonly IWorkingCalendarService workingCalendarService;
        private readonly IShiftService shiftService;
        private readonly ICommonDataService commonDataService;
        private readonly ITeamService teamService;

        public CalendarController(
            ISessionService sessionService,
            IWorkingCalendarService workingCalendarService,
            IShiftService shiftService,
            ICommonDataService commonDataService,
            ITeamService teamService) : base(sessionService)
        {
            this.workingCalendarService = workingCalendarService;
            this.shiftService = shiftService;
            this.commonDataService = commonDataService;
            this.teamService = teamService;
        }

        public ActionResult GetShift(string query, bool isShowOff = false)
        {
            var shifts = shiftService.GetByQuery(query.Trim(), isShowOff);

            return Json(shifts, JsonRequestBehavior.AllowGet);
        }

        public ActionResult MyWorkingSchedule()
        {
            return View();
        }

        public ActionResult GetWorkingSchedule(DateTime date)
        {
            var criteria = new GetEmployeeCalendarByMonthCriteria
            {
                EmpId = sessionService.Employee.Id,
                FromDate = date,
                ToDate = date.AddMonths(1).AddDays(-1)
            };
            IEnumerable<DailySchedule> workingSchedule = workingCalendarService.GetWorkingSchedule(criteria);

            return new CustomJsonResult { Data = workingSchedule, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public ActionResult WorkingCalendars()
        {
            var model = new WorkingCalendarFilterViewModel
            {
                FromDate = DateTime.Now,
                ToDate = DateTime.Now.AddDays(6),
                TeamIds = string.Join(",", teamService.GetAll().Select(x => x.Id))
            };

            return View(model);
        }

        public ActionResult WorkingCalendarDetails(WorkingCalendarFilterViewModel model)
        {
            var criteria = new GetWorkingCalendarWeeklyCriteria
            {
                EmpId = model.EmployeeId,
                Fromtime = model.FromDate,
                Totime = model.ToDate,
                TeamIds = model.TeamIds,
                JobTitleId = model.JobTitleIds,
                IsLocal = model.IsLocal
            };
            IEnumerable<WorkingCalendar> workingCalendars = workingCalendarService.GetWorkingCalendars(criteria);

            var workingCalendarDetails = new WorkingCalendarViewModel
            {
                FromDate = model.FromDate,
                ToDate = model.ToDate,
                WorkingCalendars = workingCalendars
            };

            return PartialView("_WorkingCalendarDetails", workingCalendarDetails);
        }

        public CustomJsonResult GetJobTitle()
        {
            IEnumerable<MultiSelectViewModel> jobTitles = commonDataService.GetLookupDataByCode(Constants.JobTitle).Select(x => new MultiSelectViewModel
            {
                Id = x.Id,
                Text = x.Name,
                IsLeaf = true
            }).ToList();

            return new CustomJsonResult { Data = jobTitles, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
    }
}