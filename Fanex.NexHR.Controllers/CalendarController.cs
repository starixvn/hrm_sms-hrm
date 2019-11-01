namespace Fanex.NexHR.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Web.Mvc;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Constants;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Core.Filters;
    using Fanex.NexHR.Core.Services;
    using Fanex.NexHR.Criteria.Calendar;
    using Fanex.NexHR.Criteria.Shift;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.ScheduleArrange;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.ViewModels;

    public class CalendarController : BaseController
    {
        private readonly IWorkingCalendarService workingCalendarService;
        private readonly IShiftService shiftService;
        private readonly ICommonDataService commonDataService;
        private readonly IEmployeeService employeeService;

        public CalendarController(
            ISessionService sessionService,
            IWorkingCalendarService workingCalendarService,
            IShiftService shiftService,
            ICommonDataService commonDataService,
            IEmployeeService employeeService) : base(sessionService)
        {
            this.workingCalendarService = workingCalendarService;
            this.shiftService = shiftService;
            this.commonDataService = commonDataService;
            this.employeeService = employeeService;
        }

        [HRMPermission(Policy.Read, FunctionKey.ScheduleArrange)]
        public ActionResult ShiftStatistic()
        {
            var shiftStatisticViewModel = new ShiftStatisticViewModel
            {
                FromDate = DateTime.Now.FirstDayOfWeek(),
                ToDate = DateTime.Now.LastDayOfWeek()
            };

            return View(shiftStatisticViewModel);
        }

        [HRMPermission(Policy.Read, FunctionKey.ScheduleArrange)]
        public ActionResult ShiftStatisticContent(DateTime fromDate, DateTime toDate, string groupIds, bool isShowWholeCompany, byte? isLocal)
        {
            var criteria = new GetShiftStatisticCriteria
            {
                SupervisorId = sessionService.Employee.Id,
                Fromtime = fromDate,
                Totime = toDate,
                GroupIds = groupIds,
                IsShowWholeCompany = isShowWholeCompany,
                IsLocal = isLocal
            };
            IList<ShiftStatisticModel> shiftStatisticModels = workingCalendarService.GetShiftStatistic(criteria);

            var shiftStatisticContentViewModel = new ShiftStatisticContentViewModel
            {
                FromDate = fromDate,
                ToDate = toDate,
                ShiftStatisticModels = shiftStatisticModels,
                GroupIds = groupIds,
                IsShowWholeCompany = isShowWholeCompany,
                IsLocal = isLocal
            };

            return View(shiftStatisticContentViewModel);
        }

        [HRMPermission(Policy.Read, FunctionKey.ScheduleArrange)]
        public ActionResult EmployeeShift(
            DateTime? workDate,
            int? shiftId,
            string teamIds,
            bool? isShowWholeCompany,
            byte? isLocal = null)
        {
            var model = new EmployeeShiftViewModel
            {
                TeamIds = teamIds ?? string.Empty,
                WorkDate = workDate ?? DateTime.Now,
                IsLocal = isLocal,
                IsShowWholeCompany = isShowWholeCompany.HasValue && isShowWholeCompany.Value,
                ShiftId = shiftId ?? -1,
                Shifts = shiftService.GetAll(),
                Teams = commonDataService.GetAllSection()
            };

            return View(model);
        }

        [HRMPermission(Policy.Read, FunctionKey.ScheduleArrange)]
        public ActionResult EmployeeShiftContent(
            DateTime workDate,
            int? shiftId,
            string teamIds,
            int? employeeId,
            bool? isShowWholeCompany,
            byte? isLocal = null)
        {
            var criteria = new GetEmployeeShiftDateCriteria
            {
                WorkDate = workDate,
                ShiftId = shiftId,
                EmployeeId = employeeId,
                SupervisorId = sessionService.Employee.Id,
                IsShowWholeCompany = isShowWholeCompany.HasValue && isShowWholeCompany.Value,
                TeamIds = teamIds,
                IsLocal = isLocal
            };

            IList<EmployeeShiftModel> employeeShifts = workingCalendarService.GetEmployeeShift(criteria);

            return View(employeeShifts);
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
            IList<SelectListItem> jobTitles = commonDataService.GetLookupDataByCode(Constants.JobTitle)
                .Select(j => new SelectListItem
                {
                    Text = j.Name,
                    Value = j.Id.ToString(),
                    Selected = string.IsNullOrEmpty(j.Id.ToString())
                }).ToList();

            ViewBag.HasPermissionToViewHoliday = sessionService.HasPermissionOnFunction(FunctionKey.Holiday, Policy.Read);
            ViewBag.HasPermissionToViewShift = sessionService.HasPermissionOnFunction(FunctionKey.ShiftManagement, Policy.Read);

            var model = new WorkingCalendarFilterViewModel
            {
                FromDate = DateTime.Now.FirstDayOfWeek(),
                ToDate = DateTime.Now.LastDayOfWeek(),
                JobTitles = jobTitles
            };

            return View(model);
        }

        public ActionResult WorkingCalendarDetails(WorkingCalendarFilterViewModel model)
        {
            IList<WorkingCalendarsViewModel> workingCalendars;
            int month = DateTime.Now.Month;
            int year = DateTime.Now.Year;
            var firstDateInMonth = new StringBuilder();

            firstDateInMonth.AppendFormat("{0}/{1}/1", year, month);

            var getUserChangeShiftCriteria = new GetUserChangedShiftCriteria
            {
                ReportDate = new DateTime(year, month, 1)
            };
            var getUserHalfDayOffsCriteria = new GetEmployeeHalfDayOffCriteria
            {
                ReportDate = DateTime.Parse(firstDateInMonth.ToString())
            };
            IList<UserChangedShift> userChangedShifts = employeeService.GetUserChangedShift(getUserChangeShiftCriteria);
            IList<HalfDayOff> userHalfDayOffs = employeeService.GetHalfDayOffs(getUserHalfDayOffsCriteria);

            var criteria = new GetWorkingCalendarWeeklyCriteria
            {
                EmpId = model.EmployeeId,
                Fromtime = model.FromDate,
                Totime = model.ToDate,
                GroupIds = model.SectionIds,
                JobTitleId = model.JobTitleIds,
                IsLocal = model.IsLocal
            };

            workingCalendars = workingCalendarService.GetWorkingCalendars(criteria);
            CheckChangeShiff(workingCalendars, userChangedShifts, model.FromDate);
            CheckHalfDayOff(workingCalendars, userHalfDayOffs, model.FromDate);

            var workingCalendarDetails = new WorkingCalendarDetailViewModel
            {
                Year = year,
                Month = month,
                FromDate = model.FromDate,
                ToDate = model.ToDate,
                WorkingCalendars = workingCalendars
            };

            return PartialView("_WorkingCalendarDetails", workingCalendarDetails);
        }

        private static void CheckChangeShiff(IList<WorkingCalendarsViewModel> models, IList<UserChangedShift> userChangeShift, DateTime fromDate)
        {
            foreach (UserChangedShift user in userChangeShift)
            {
                HandleUserChangeShift(user, models, fromDate);
            }
        }

        private static void HandleUserChangeShift(UserChangedShift user, IList<WorkingCalendarsViewModel> models, DateTime fromDate)
        {
            foreach (WorkingCalendarsViewModel employee in models.Where(x => x.EmpId == user.EmpId))
            {
                for (int i = 0; i < employee.WorkingDays.Count; i++)
                {
                    if (user.Day == fromDate.AddDays(i).Day)
                    {
                        var changeShift = "_" + employee.WorkingDays[i];
                        employee.WorkingDays[i] = changeShift;
                        employee.WorkingSchedule[i].Title = changeShift;
                    }
                }
            }
        }

        private static void CheckHalfDayOff(IList<WorkingCalendarsViewModel> models, IList<HalfDayOff> halfDayOff, DateTime fromDate)
        {
            foreach (HalfDayOff item in halfDayOff)
            {
                HandleHalfDayOff(item, models, fromDate);
            }
        }

        private static void HandleHalfDayOff(HalfDayOff halfDayOff, IList<WorkingCalendarsViewModel> models, DateTime fromDate)
        {
            foreach (WorkingCalendarsViewModel employee in models.Where(x => x.EmpId == halfDayOff.Id))
            {
                for (int i = 0; i < employee.WorkingDays.Count; i++)
                {
                    if (halfDayOff.Day == fromDate.AddDays(i).Day)
                    {
                        var halfDay = "-" + employee.WorkingDays[i];
                        employee.WorkingDays[i] = halfDay;
                        employee.WorkingSchedule[i].Title = halfDay;
                    }
                }
            }
        }
    }
}