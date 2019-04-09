namespace Fanex.NexHR.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Web.Mvc;
    using Core.Models;
    using Core.Services;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Constants;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Criteria.Calendar;
    using Fanex.NexHR.Criteria.Lookup;
    using Fanex.NexHR.Criteria.Shift;
    using Fanex.NexHR.Criteria.Supervisor;
    using Fanex.NexHR.Criteria.Team;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Supervisor;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.ViewModels;
    using Fanex.NexHR.Web.Providers;

    public class CalendarController : BaseController
    {
        private readonly IWorkingCalendarService workingCalendarService;
        private readonly ITeamService teamService;
        private readonly IShiftService shiftService;
        private readonly ILookupDataService lookupDataService;
        private readonly IEmployeeService employeeService;
        private readonly IBaseService baseService;

        public CalendarController(
            IWorkingCalendarService workingCalendarService,
            ITeamService teamService,
            IShiftService shiftService,
            ILookupDataService lookupDataService,
            IEmployeeService employeeService,
            IBaseService baseService)
        {
            this.workingCalendarService = workingCalendarService;
            this.teamService = teamService;
            this.shiftService = shiftService;
            this.lookupDataService = lookupDataService;
            this.employeeService = employeeService;
            this.baseService = baseService;
        }

        [HttpGet]
        [HRAuthorize(Policy.Read, FunctionKey.ScheduleArrange)]
        public ActionResult ArrangeSchedule()
        {
            ViewBag.IsSARole = CurrentUser.Roles.Any(i => i.RoleId == (int)RoleSystem.SystemAdmin);
            ViewBag.IsHost = CurrentUser.HasPermission(FunctionKey.ScheduleArrange, Policy.Host);

            return View();
        }

        [HRAuthorize(Policy.Read, FunctionKey.ScheduleArrange)]
        public ActionResult ShiftStatistic()
        {
            var currentDate = DateTime.Now;
            var fromDate = GetStartOfWeek(currentDate);
            var toDate = GetEndOfWeek(currentDate);
            var getAllGroupCriteria = new GetAllGroupCriteria();
            IList<Team> teams = teamService.GetAll(getAllGroupCriteria);

            var shiftStatisticViewModel = new ShiftStatisticViewModel
            {
                FromDate = fromDate,
                ToDate = toDate,
                Teams = teams
            };

            return View(shiftStatisticViewModel);
        }

        [HRAuthorize(Policy.Read, FunctionKey.ScheduleArrange)]
        public ActionResult ShiftStatisticContent(DateTime fromDate, DateTime toDate, string groupIds, bool isShowWholeCompany, byte? isLocal)
        {
            var getShiftCriteria = new GetShiftStatisticCriteria
            {
                SupervisorId = CurrentUser.Id,
                Fromtime = fromDate,
                Totime = toDate,
                GroupIds = groupIds,
                IsShowWholeCompany = isShowWholeCompany,
                IsLocal = isLocal
            };
            var shiftStatisticModels = workingCalendarService.GetShiftStatistic(getShiftCriteria);

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

        [HRAuthorize(Policy.Read, FunctionKey.ScheduleArrange)]
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
                WorkDate = workDate.HasValue ? workDate.Value : DateTime.Now,
                IsLocal = isLocal,
                IsShowWholeCompany = isShowWholeCompany.HasValue && isShowWholeCompany.Value,
                ShiftId = shiftId.HasValue ? shiftId.Value : -1,
                Shifts = shiftService.GetAll()
            };

            var getAllGroupCriteria = new GetAllGroupCriteria();
            model.Teams = teamService.GetAll(getAllGroupCriteria);

            return View(model);
        }

        [HRAuthorize(Policy.Read, FunctionKey.ScheduleArrange)]
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
                SupervisorId = CurrentUser.Id,
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

        public ActionResult GetWorkingSchedule(DateTime date, int id)
        {
            var workingScheduleCriteria = new GetEmployeeCalendarByMonthCriteria
            {
                ReportDate = date,
                EmpId = id
            };
            var workingSchedule = workingCalendarService.GetWorkingSchedule(workingScheduleCriteria);

            return Json(workingSchedule, JsonRequestBehavior.AllowGet);
        }

        public ActionResult WorkingCalendars()
        {
            IList<Team> sections = teamService.GetAll(new GetAllGroupCriteria());

            var getAllJobTitleCriteria = new GetAllLookupDataCriteria { LookupCode = Constants.JobTitle };
            IList<SelectListItem> jobTitles = lookupDataService.GetAll(getAllJobTitleCriteria).Select(j => new SelectListItem
            {
                Text = j.Name,
                Value = j.Id.ToString(),
                Selected = string.IsNullOrEmpty(j.Id.ToString())
            }).ToList();

            ViewBag.HasPermissionToViewHoliday = CurrentUser.HasPermission(FunctionKey.Holiday, Policy.Read);
            ViewBag.HasPermissionToViewShift = CurrentUser.HasPermission(FunctionKey.ShiftManagement, Policy.Read);

            var model = new WorkingCalendarFilterViewModel
            {
                FromDate = GetStartOfWeek(DateTime.Now),
                ToDate = GetEndOfWeek(DateTime.Now),
                Sections = sections,
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

        public ActionResult MemberScheduleInOrder()
        {
            var criteria = new GetEmployeeByShiftManagementCriteria
            {
                EmpId = CurrentUser.Id,
            };
            IList<SupervisorModel> shiftManagerModel = workingCalendarService.GetEmployeeByShiftManagement(criteria);

            var model = new ShiftManagerViewModel
            {
                ShiftManagerList = shiftManagerModel,
                EmployeeId = CurrentUser.Id,
                SupervisorRole = (short)RoleSystem.ShiftManager
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult MemberScheduleInOrder(string employeeOrders)
        {
            var employeeOrdersDetail = JsonConverter.FromJsonArray<MemberOrderModel>(employeeOrders);

            var criteria = new UpdateOrderShiftArrangeCriteria
            {
                MemberOrders = employeeOrdersDetail,
                SupervisorId = CurrentUser.Id
            };
            CommandResult result = baseService.ExecuteNonQuery(criteria);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        private static DateTime GetStartOfWeek(DateTime currentDate)
        {
            int subDays = currentDate.DayOfWeek - DayOfWeek.Monday;
            if (subDays < 0)
            {
                subDays += 7;
            }

            return currentDate.AddDays(-1 * subDays).Date;
        }

        private static DateTime GetEndOfWeek(DateTime currentDate)
        {
            return GetStartOfWeek(currentDate).AddDays(6);
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