namespace Fanex.NexHR.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Web.Mvc;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Criteria.Calendar;
    using Fanex.NexHR.Criteria.Lookup;
    using Fanex.NexHR.Criteria.Shift;
    using Fanex.NexHR.Criteria.Supervisor;
    using Fanex.NexHR.Criteria.Team;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Supervisor;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interfaces;
    using Fanex.NexHR.ViewModels;
    using Fanex.NexHR.Web.Providers;
    using Nexus.RoleManagement.Services;

    public class CalendarController : BaseController
    {
        private readonly IWorkingCalendarService workingCalendarService;
        private readonly ITeamService teamService;
        private readonly IShiftService shiftService;
        private readonly ILookupDataService lookupDataService;
        private readonly IEmployeeService employeeService;
        private readonly ICommandService commandService;

        public CalendarController()
            : this(null, null, null, null, null, null)
        {
        }

        public CalendarController(
            IWorkingCalendarService workingCalendarService,
            ITeamService teamService,
            IShiftService shiftService,
            ILookupDataService lookupDataService,
            IEmployeeService employeeService,
            ICommandService commandService)
        {
            this.workingCalendarService = workingCalendarService ?? new WorkingCalendarService();
            this.teamService = teamService ?? new TeamService();
            this.shiftService = shiftService ?? new ShiftService();
            this.lookupDataService = lookupDataService ?? new LookupDataService();
            this.employeeService = employeeService ?? new EmployeeService();
            this.commandService = commandService ?? new CommandService();
        }

        [HttpGet]
        [HRAuthorize(Policy.Read, FunctionKey.ScheduleArrange)]
        public ActionResult ArrangeSchedule()
        {
            var currentDate = DateTime.Now;
            DateTime fromDate = GetStartOfWeek(currentDate),
                toDate = GetEndOfWeek(currentDate);

            IList<Team> teams = teamService.GetAll(new GetAllGroupCriteria());
            var getAllJobTitleCriteria = new GetAllLookupDataCriteria
            {
                LookupCode = Constants.JobTitle
            };
            IList<LookupData> jobTitles = lookupDataService.GetAll(getAllJobTitleCriteria);
            var model = new ScheduleArrangeFilterViewModel
            {
                FromDate = fromDate,
                ToDate = toDate,
                Groups = teams,
                JobTitles = jobTitles
            };

            ViewBag.HasPermission = CurrentUser.HasPermission(FunctionKey.ScheduleArrange, Policy.Host);

            return View(model);
        }

        [HttpPost]
        [HRAuthorize(Policy.Host, FunctionKey.ScheduleArrange)]
        public ActionResult ArrangeSchedule(string scheduleArranges, string shiftMultiples)
        {
            var scheduleArrangeDetail = JsonConverter.FromJsonArray<ScheduleModel>(scheduleArranges);
            var shiftMultipleDetail = JsonConverter.FromJsonArray<ShiftMultipleModel>(shiftMultiples);
            var scheduleArrangeModel = new ScheduleArrangeModel
            {
                ScheduleModels = scheduleArrangeDetail,
                ShiftMultipleModels = shiftMultipleDetail,
                ModifiedBy = CurrentUser.Id,
                ModifiedDate = DateTime.Now,
            };

            var criteria = new InsertOrUpdateWorkingCalendarsCriteria()
            {
                ModifiedDate = scheduleArrangeModel.ModifiedDate,
                ModifiedBy = scheduleArrangeModel.ModifiedBy,
                ScheduleList = scheduleArrangeModel.ScheduleModelXML,
                ShiftMultipleList = scheduleArrangeModel.ShiftMultipleXML
            };
            CommandResult result = commandService.Insert(criteria);

            return Json(new { errorCode = result.Code, message = result.Message }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [HRAuthorize(Policy.Read, FunctionKey.ScheduleArrange)]
        public ActionResult ArrangeScheduleDetail(DateTime fromDate, DateTime toDate, string groupIds, string jobTitles, int? employeeId = null)
        {
            var getWorkingCalendarCriteria = new GetWorkingCalendarsForShiftArrangeCriteria()
            {
                SupervisorID = CurrentUser.Id,
                Fromtime = fromDate,
                Totime = toDate,
                GroupIds = groupIds,
                JobTitleId = jobTitles,
                EmpID = employeeId
            };
            IList<WorkingCalendarModel> workingCalendars = workingCalendarService.GetWorkingCalendarsForShiftArrange(getWorkingCalendarCriteria);
            var scheduleArranges = new List<ScheduleArrangeViewModel>();

            if (workingCalendars.Any())
            {
                scheduleArranges = ConvertWorkingCalendarToScheduleModel(fromDate, toDate, workingCalendars).ToList();
            }

            var scheduleViewModel = new ScheduleViewModel
            {
                FromDate = fromDate,
                ToDate = toDate,
                ScheduleArranges = scheduleArranges
            };

            ViewBag.IsSARole = CurrentUser.Roles.Any(i => i.RoleId == (int)RoleSystem.SystemAdmin);
            ViewBag.HasPermission = CurrentUser.HasPermission(FunctionKey.ScheduleArrange, Policy.Host);

            return View(scheduleViewModel);
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
            var getShiftCriteria = new GetShiftStatisticCriteria()
            {
                SupervisorID = CurrentUser.Id,
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
                TeamIds = teamIds == null ? string.Empty : teamIds,
                WorkDate = workDate.HasValue ? workDate.Value : DateTime.Now,
                IsLocal = isLocal,
                IsShowWholeCompany = isShowWholeCompany.HasValue ? isShowWholeCompany.Value : false,
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
            var criteria = new GetEmployeeShiftDateCriteria()
            {
                WorkDate = workDate,
                ShiftId = shiftId,
                EmployeeId = employeeId,
                SupervisorID = CurrentUser.Id,
                IsShowWholeCompany = isShowWholeCompany.HasValue ? isShowWholeCompany.Value : false,
                TeamIds = teamIds,
                IsLocal = isLocal
            };

            IList<EmployeeShiftModel> employeeShifts = workingCalendarService.GetEmployeeShift(criteria);

            return View(employeeShifts);
        }

        public ActionResult GetEmployeesByShiftMonitor(string query)
        {
            var criteria = new GetEmployeeSupervisorByQueryCriteria()
            {
                EmpId = this.CurrentUser.Id,
                Role = (int)RoleSystem.ShiftManager,
                RequestType = null,
                Query = query.Trim(),
                IncludeLoginUser = false
            };

            IList<SupervisorNameModel> employees = employeeService.GetSupervisorByQuery(criteria);
            return Json(employees, JsonRequestBehavior.AllowGet);
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
            var workingScheduleCriteria = new GetEmployeeCalendarByMonthCriteria()
            {
                ReportDate = date,
                EmpId = id
            };
            var workingSchedule = workingCalendarService.GetWorkingSchedule(workingScheduleCriteria);

            return Json(workingSchedule, JsonRequestBehavior.AllowGet);
        }

        public ActionResult WorkingCalendars()
        {
            var getAllGroupCriteria = new GetAllGroupCriteria();
            var getAllJobTitleCriteria = new GetAllLookupDataCriteria { LookupCode = Constants.JobTitle };
            var jobTitles = lookupDataService.GetAll(getAllJobTitleCriteria).Select(j => new SelectListItem
            {
                Text = j.Name,
                Value = j.Id.ToString(),
                Selected = string.IsNullOrEmpty(j.Id.ToString())
            }).ToList();

            ViewBag.HasPermissionToViewHoliday = CurrentUser.HasPermission(FunctionKey.Holiday, Policy.Read);
            ViewBag.HasPermissionToViewShift = CurrentUser.HasPermission(FunctionKey.ShiftManagement, Policy.Read);

            var model = new WorkingCalendarFilterViewModel()
            {
                FromDate = GetStartOfWeek(DateTime.Now),
                ToDate = GetEndOfWeek(DateTime.Now),
                Groups = teamService.GetAll(getAllGroupCriteria),
                JobTitles = jobTitles
            };

            return View(model);
        }

        public ActionResult WorkingCalendarDetails(int? empId, DateTime fromDate, DateTime toDate, string groupId, string jobTitleId, byte? isLocal = null)
        {
            var workingCalendars = new List<WorkingCalendarsViewModel>();
            int month = DateTime.Now.Month;
            int year = DateTime.Now.Year;
            var firstDateInMonth = new StringBuilder();

            firstDateInMonth.AppendFormat("{0}/{1}/1", year, month);

            var getUserChangeShiftCriteria = new GetUserChangedShiftCriteria()
            {
                ReportDate = new DateTime(year, month, 1)
            };
            var getUserHalfDayOffsCriteria = new GetEmployeeHalfDayOffCriteria()
            {
                ReportDate = DateTime.Parse(firstDateInMonth.ToString())
            };
            IList<UserChangedShift> userChangedShifts = employeeService.GetUserChangedShift(getUserChangeShiftCriteria);
            IList<HalfDayOff> userHalfDayOffs = employeeService.GetHalfDayOffs(getUserHalfDayOffsCriteria);

            var criteria = new GetWorkingCalendarWeeklyCriteria()
            {
                EmpId = empId,
                Fromtime = fromDate,
                Totime = toDate,
                GroupIds = groupId,
                JobTitleId = jobTitleId,
                IsLocal = isLocal
            };

            workingCalendars = workingCalendarService.GetWorkingCalendars(criteria).ToList();
            CheckChangeShiff(workingCalendars, userChangedShifts, fromDate);
            CheckHalfDayOff(workingCalendars, userHalfDayOffs, fromDate);

            var model = new WorkingCalendarDetailViewModel()
            {
                Year = year,
                Month = month,
                FromDate = fromDate,
                ToDate = toDate,
                WorkingCalendars = workingCalendars
            };

            return PartialView(model);
        }

        public ActionResult MemberScheduleInOrder()
        {
            var criteria = new GetEmployeeByShiftManagementCriteria()
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

            var criteria = new UpdateOrderShiftArrangeCriteria()
            {
                MemberOrders = employeeOrdersDetail,
                SupervisorId = CurrentUser.Id
            };
            CommandResult result = commandService.Update(criteria);

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
        }

        private static void CheckHalfDayOff(IList<WorkingCalendarsViewModel> models, IList<HalfDayOff> halfDayOff, DateTime fromDate)
        {
            foreach (HalfDayOff item in halfDayOff)
            {
                foreach (WorkingCalendarsViewModel employee in models.Where(x => x.EmpId == item.ID))
                {
                    for (int i = 0; i < employee.WorkingDays.Count; i++)
                    {
                        if (item.Day == fromDate.AddDays(i).Day)
                        {
                            var halfDay = "-" + employee.WorkingDays[i];
                            employee.WorkingDays[i] = halfDay;
                            employee.WorkingSchedule[i].Title = halfDay;
                        }
                    }
                }
            }
        }

        [NonAction]
        private IList<ScheduleArrangeViewModel> ConvertWorkingCalendarToScheduleModel(DateTime fromDate, DateTime toDate, IList<WorkingCalendarModel> workingCalendars)
        {
            var scheduleModels = new List<ScheduleArrangeViewModel>();
            var numberDays = (toDate - fromDate).Days + 1;
            IEnumerable<WorkingCalendarModel> employeeWorkingCalendars = null;
            WorkingCalendarModel workingCalendar = null;
            ScheduleArrangeViewModel scheduleArrangeModel = null;
            var workingDate = DateTime.Now.Date;
            var employees = workingCalendars.GroupBy(x => x.Employee.Id).Select(x => x.Key);

            foreach (var employeeId in employees)
            {
                var scheduleHasInfo = false;
                for (int i = 0; i < numberDays; i++)
                {
                    workingDate = fromDate.AddDays(i);
                    employeeWorkingCalendars = workingCalendars.Where(t => (t.WorkDate.Date == workingDate && t.Employee.Id == employeeId));

                    if (employeeWorkingCalendars != null && employeeWorkingCalendars.Any())
                    {
                        workingCalendar = employeeWorkingCalendars.FirstOrDefault();

                        if (!scheduleHasInfo)
                        {
                            scheduleArrangeModel = new ScheduleArrangeViewModel
                            {
                                EmployeeId = workingCalendar.Employee.Id,
                                NickName = workingCalendar.Employee.NickName,
                                TeamName = workingCalendar.Employee.Team.Name,
                                TeamCode = workingCalendar.Employee.Team.Code
                            };

                            scheduleHasInfo = true;
                        }

                        SetWorkingCalendarForDayOfWeek(scheduleArrangeModel, workingCalendar);
                    }
                }

                scheduleModels.Add(scheduleArrangeModel);
            }

            return scheduleModels;
        }

        [NonAction]
        private ScheduleArrangeViewModel SetWorkingCalendarForDayOfWeek(ScheduleArrangeViewModel scheduleModel, WorkingCalendarModel workingCalendar)
        {
            switch (workingCalendar.Weekday - 1)
            {
                case (int)DayOfWeek.Monday:
                    scheduleModel.MondaySchedule = workingCalendar;
                    break;

                case (int)DayOfWeek.Tuesday:
                    scheduleModel.TuesdaySchedule = workingCalendar;
                    break;

                case (int)DayOfWeek.Wednesday:
                    scheduleModel.WednesdaySchedule = workingCalendar;
                    break;

                case (int)DayOfWeek.Thursday:
                    scheduleModel.ThursdaySchedule = workingCalendar;
                    break;

                case (int)DayOfWeek.Friday:
                    scheduleModel.FridaySchedule = workingCalendar;
                    break;

                case (int)DayOfWeek.Saturday:
                    scheduleModel.SaturdaySchedule = workingCalendar;
                    break;

                case (int)DayOfWeek.Sunday:
                    scheduleModel.SundaySchedule = workingCalendar;
                    break;
            }

            return scheduleModel;
        }
    }
}