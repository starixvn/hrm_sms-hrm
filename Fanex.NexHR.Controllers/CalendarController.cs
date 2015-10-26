// --------------------------------------------------------------------------
// <copyright file="CalendarController.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.11.18">Created</change>
// </history>

namespace Fanex.NexHR.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Web.Mvc;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Criteria.Calendar;
    using Fanex.NexHR.Criteria.Lookup;
    using Fanex.NexHR.Criteria.Shift;
    using Fanex.NexHR.Criteria.Supervisor;
    using Fanex.NexHR.Criteria.Team;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Calendar;
    using Fanex.NexHR.Models.Supervisor;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interfaces;
    using Fanex.NexHR.ViewModels.Calendar;
    using Fanex.NexHR.ViewModels.Supervisor;

    [Authorize]
    [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Reviewed. Suppression is OK here.")]
    public class CalendarController : BaseController
    {
        private readonly IWorkingCalendarService _workingCalendarService;
        private readonly ITeamService _teamService;
        private readonly IShiftService _shiftService;
        private readonly ILookupDataService _lookupDataService;
        private readonly IEmployeeService _employeeService;

        public CalendarController()
            : this(null, null, null, null, null)
        {
        }

        public CalendarController(
            IWorkingCalendarService workingCalendarService, 
            ITeamService teamService, 
            IShiftService shiftService,
            ILookupDataService lookupDataService = null,
            IEmployeeService employeeService = null)
        {
            _workingCalendarService = workingCalendarService ?? new WorkingCalendarService();
            _teamService = teamService ?? new TeamService();
            _shiftService = shiftService ?? new ShiftService();
            _lookupDataService = lookupDataService ?? new LookupDataService();
            _employeeService = employeeService ?? new EmployeeService();
        }

        [HttpGet]
        [Authorize(Roles = "ShiftManager")]
        public ActionResult ArrangeSchedule()
        {
            var currentDate = DateTime.Now;
            var fromDate = GetStartOfWeek(currentDate);
            var toDate = GetEndOfWeek(currentDate);
            var getAllGroupCriteria = new GetAllGroupCriteria();
            IList<Team> teams = _teamService.GetAll(getAllGroupCriteria);
            var getAllJobTitleCriteria = new GetAllLookupDataCriteria
            {
                LookupCode = Constants.JobTitle
            };
            IList<LookupData> jobTitles = _lookupDataService.GetAll(getAllJobTitleCriteria);
            var scheduleArrangeViewModel = new ScheduleArrangeFilterViewModel
            {
                FromDate = fromDate,
                ToDate = toDate,
                Groups = teams,
                JobTitles = jobTitles
            };

            return View(scheduleArrangeViewModel);
        }

        [HttpPost]
        [Authorize(Roles = "ShiftManager")]
        public ActionResult ArrangeSchedule(string scheduleArranges, string shiftMultiples)
        {
            var scheduleArrangeDetail = JsonConverter.FromJsonArray<ScheduleModel>(scheduleArranges);
            var shiftMultipleDetail = JsonConverter.FromJsonArray<ShiftMultipleModel>(shiftMultiples);
            var scheduleArrangeModel = new ScheduleArrangeModel
            {
                ScheduleModels = scheduleArrangeDetail,
                ShiftMultipleModels = shiftMultipleDetail,
                ModifiedBy = this.CurrentUser.Id,
                ModifiedDate = DateTime.Now,
            };

            var criteria = new InsertOrUpdateWorkingCalendarsCriteria()
            {
                ModifiedDate = scheduleArrangeModel.ModifiedDate,
                ModifiedBy = scheduleArrangeModel.ModifiedBy,
                ScheduleList = scheduleArrangeModel.ScheduleModelXML,
                ShiftMultipleList = scheduleArrangeModel.ShiftMultipleXML
            };
            CommandResult result = _workingCalendarService.InsertOrUpdateWorkingCalendars(criteria);

            return Json(new { errorCode = result.Code, message = result.Message }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Authorize(Roles = "ShiftManager")]
        public ActionResult ArrangeScheduleDetail(DateTime fromDate, DateTime toDate, string groupIds, string jobTitles, int? employeeId = null)
        {
            var getWorkingCalendarCriteria = new GetWorkingCalendarsForShiftArrangeCriteria()
            {
                SupervisorID = this.CurrentUser.Id,
                Fromtime = fromDate,
                Totime = toDate,
                GroupIds = groupIds,
                JobTitleId = jobTitles,
                EmpID = employeeId
            };
            var workingCalendars = _workingCalendarService.GetWorkingCalendarsForShiftArrange(getWorkingCalendarCriteria);
            var scheduleArranges = ConvertWorkingCalendarToScheduleModel(fromDate, toDate, workingCalendars);

            var scheduleViewModel = new ScheduleViewModel
            {
                FromDate = fromDate,
                ToDate = toDate,
                ScheduleArranges = scheduleArranges
            };

            return View(scheduleViewModel);
        }

        [Authorize(Roles = "ShiftManager,HR")]
        public ActionResult ShiftStatistic()
        {
            var currentDate = DateTime.Now;
            var fromDate = GetStartOfWeek(currentDate);
            var toDate = GetEndOfWeek(currentDate);
            var getAllGroupCriteria = new GetAllGroupCriteria();
            IList<Team> teams = _teamService.GetAll(getAllGroupCriteria);

            var shiftStatisticViewModel = new ShiftStatisticViewModel
            {
                FromDate = fromDate,
                ToDate = toDate,
                Teams = teams
            };

            return View(shiftStatisticViewModel);
        }

        [Authorize(Roles = "ShiftManager,HR")]
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
            var shiftStatisticModels = _workingCalendarService.GetShiftStatistic(getShiftCriteria);

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

        [Authorize(Roles = "ShiftManager,HR")]
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
                Shifts = _shiftService.GetAll()
            };

            var getAllGroupCriteria = new GetAllGroupCriteria();
            model.Teams = _teamService.GetAll(getAllGroupCriteria);

            return View(model);
        }

        [Authorize(Roles = "ShiftManager,HR")]
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

            IList<EmployeeShiftModel> employeeShifts = _workingCalendarService.GetEmployeeShift(criteria);

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

            IList<SupervisorNameModel> employees = _employeeService.GetSupervisorByQuery(criteria);
            return Json(employees, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetShift(string query, bool isShowOff = false)
        {
            var shifts = _shiftService.GetByQuery(query.Trim(), isShowOff);

            return Json(shifts, JsonRequestBehavior.AllowGet);
        }

        public ActionResult MyWorkingSchedule()
        {
            var workingScheduleCriteria = new GetEmployeeCalendarByMonthCriteria()
            {
                ReportDate = DateTime.Now,
                EmpId = this.CurrentUser.Id
            };
            IList<WorkingScheduleViewModel> workingSchedule = _workingCalendarService.GetWorkingSchedule(workingScheduleCriteria);

            return View(workingSchedule);
        }

        public ActionResult GetWorkingSchedule(DateTime date, int id)
        {
            var workingScheduleCriteria = new GetEmployeeCalendarByMonthCriteria()
            {
                ReportDate = date,
                EmpId = id
            };
            var workingSchedule = _workingCalendarService.GetWorkingSchedule(workingScheduleCriteria);

            return Json(workingSchedule, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Workings the calendars.
        /// </summary>
        /// <returns>ActionResult return View Working Calendars.</returns>
        public ActionResult WorkingCalendars()
        {
            var currentDate = DateTime.Now;
            var fromDate = GetStartOfWeek(currentDate);
            var toDate = GetEndOfWeek(currentDate);
            ViewBag.FromDate = fromDate;
            ViewBag.ToDate = toDate;
            ViewBag.JobTitleId = string.Empty;

            var getAllGroupCriteria = new GetAllGroupCriteria();
            ViewBag.Groups = _teamService.GetAll(getAllGroupCriteria);
            var getAllJobTitleCriteria = new GetAllLookupDataCriteria
            {
                LookupCode = Constants.JobTitle
            };
            IList<LookupData> listJobTitle = _lookupDataService.GetAll(getAllJobTitleCriteria);
            ViewBag.JobTitles = listJobTitle.Select(j => new SelectListItem
            {
                Text = j.Name,
                Value = j.Id.ToString(),
                Selected = string.IsNullOrEmpty(j.Id.ToString())
            }).ToList();

            return View();
        }

        public ActionResult WorkingCalendarDetails(int? empId, DateTime fromDate, DateTime toDate, string groupId, string jobTitleId, byte? isLocal = null)
        {
            var viewModel = new List<WorkingCalendarsViewModel>();
            int month = 0;
            int year = 0;
            var firstDateInMonth = new System.Text.StringBuilder();
            ViewBag.Year = year != 0 ? year : DateTime.Now.Year;
            ViewBag.Month = month != 0 ? month : DateTime.Now.Month;
            firstDateInMonth.AppendFormat("{0}/{1}/1", ViewBag.Year, ViewBag.Month);

            var getUserChangeShiftCriteria = new GetUserChangedShiftCriteria()
            {
                ReportDate = new DateTime(ViewBag.Year, ViewBag.Month, 1)
            };
            var getUserHalfDayOffsCriteria = new GetEmployeeHalfDayOffCriteria()
            {
                ReportDate = DateTime.Parse(firstDateInMonth.ToString())
            };
            var userChangedShift = _employeeService.GetUserChangedShift(getUserChangeShiftCriteria);
            var userHalfDayOffs = _employeeService.GetHalfDayOffs(getUserHalfDayOffsCriteria); 
           
            ViewBag.FromNewDate = fromDate;
            ViewBag.ToNewDate = toDate;
            var criteria = new GetWorkingCalendarWeeklyCriteria()
            {
                EmpId = empId,
                Fromtime = fromDate,
                Totime = toDate,
                GroupIds = groupId,
                JobTitleId = jobTitleId,
                IsLocal = isLocal
            };

            viewModel = _workingCalendarService.GetWorkingCalendars(criteria).ToList();
            CheckChangeShiffAndHalfDayOff(fromDate, viewModel, userChangedShift, userHalfDayOffs);
            ViewBag.UserChangeShift = userChangedShift;
            ViewBag.HalfDayOffs = userHalfDayOffs;
             
            return PartialView(viewModel);
        }

        /// <summary>
        /// Action to order schedule member.
        /// </summary>
        /// <returns>Action Result.</returns>
        public ActionResult MemberScheduleInOrder()
        {
            var getEmployeeSupervisorCriteria = new GetEmployeeSupervisorCriteria()
            {
                EmpId = this.CurrentUser.Id,
                Role = (int)RoleSystem.ShiftManager
            };
            IList<SupervisorModel> shiftManagerModel = _employeeService.GetEmployeeSupervisor(getEmployeeSupervisorCriteria);

            var model = new ShiftManagerViewModel
            {
                ShiftManagerList = shiftManagerModel,
                EmployeeId = this.CurrentUser.Id,
                SupervisorRole = (short)RoleSystem.ShiftManager
            };

            return View(model);
        }

        /// <summary>
        /// Action to order schedule member.
        /// </summary>
        /// <param name="employeeOrders">Employee Orders.</param>
        /// <returns>Boolean to check.</returns>
        [HttpPost]
        public ActionResult MemberScheduleInOrder(string employeeOrders)
        {
            var errorCode = 0;
            var message = string.Empty;
            var employeeOrdersDetail = JsonConverter.FromJsonArray<MemberOrderModel>(employeeOrders);
            var memberOrders = new MemberScheduleOrderModel
            {
                SupervisorId = this.CurrentUser.Id,
                MemberOrders = employeeOrdersDetail,
                ModifiedBy = this.CurrentUser.Id
            };

            var updateOrderShiftArrangeCriteria = new UpdateOrderShiftArrangeCriteria()
            {
                EmployeeOrders = memberOrders.MemberOrderXML,
                ModifiedBy = memberOrders.ModifiedBy,
                SupervisorId = memberOrders.SupervisorId
            };
            _employeeService.UpdateOrderShiftArrange(updateOrderShiftArrangeCriteria);

            return Json(new { errorCode, message }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Function to convert WorkingCalendar To ScheduleModel.
        /// </summary>
        /// <param name="fromDate">From Date.</param>
        /// <param name="toDate">The to date.</param>
        /// <param name="workingCalendars">WorkingCalendarModel List.</param>
        /// <returns>ScheduleModel List.</returns>
        [NonAction]
        private IList<ScheduleArrangeViewModel> ConvertWorkingCalendarToScheduleModel(DateTime fromDate, DateTime toDate, IList<WorkingCalendarModel> workingCalendars)
        {
            var scheduleModels = new List<ScheduleArrangeViewModel>();
            var numberDays = (toDate - fromDate).Days + 1;
            IEnumerable<WorkingCalendarModel> tempWokingCalendars = null;
            WorkingCalendarModel tempWokingCalendar = null;
            ScheduleArrangeViewModel tempScheduleModel = null;
            var tempDate = DateTime.Now.Date;
            var tempEmpId = 0;

            foreach (var workingCalendar in workingCalendars)
            {
                if (tempEmpId != workingCalendar.Employee.Id)
                {
                    tempScheduleModel = new ScheduleArrangeViewModel
                    {
                        EmployeeId = workingCalendar.Employee.Id,
                        NickName = workingCalendar.Employee.NickName,
                        TeamName = workingCalendar.Employee.Team.Name,
                        TeamCode = workingCalendar.Employee.Team.Code
                    };

                    for (int i = 0; i < numberDays; i++)
                    {
                        tempDate = fromDate.AddDays(i);
                        tempWokingCalendars = workingCalendars.Where(t => (t.WorkDate.Date == tempDate && t.Employee.Id == workingCalendar.Employee.Id));

                        if (tempWokingCalendars != null && tempWokingCalendars.Any())
                        {
                            tempWokingCalendar = tempWokingCalendars.FirstOrDefault();
                            SetWorkingCalendarForDayOfWeek(tempScheduleModel, tempWokingCalendar);
                        }
                    }

                    scheduleModels.Add(tempScheduleModel);
                    tempEmpId = workingCalendar.Employee.Id;
                }
            }

            return scheduleModels;
        }

        /// <summary>
        /// Function to set Working Calendar For DayOfWeek.
        /// </summary>
        /// <param name="scheduleModel">Input Schedule Model.</param>
        /// <param name="workingCalendar">WorkingCalendar Model.</param>
        /// <returns>Schedule Model.</returns>
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

        [NonAction]
        private DateTime GetStartOfWeek(DateTime currentDate)
        {
            int subDays = currentDate.DayOfWeek - DayOfWeek.Monday;
            if (subDays < 0)
            {
                subDays += 7;
            }

            return currentDate.AddDays(-1 * subDays).Date;
        }

        [NonAction]
        private DateTime GetEndOfWeek(DateTime currentDate)
        {
            return GetStartOfWeek(currentDate).AddDays(6);
        }

        /// <summary>
        /// Method check User Change Shift or Half Day Off.
        /// </summary>
        /// <param name="fromDate">From date.</param>
        /// <param name="viewModel">Working Calendars ViewModel.</param>
        /// <param name="userChangeShift">UserChangeShift model.</param>
        /// <param name="halfDayOff">HalfDayOff model.</param>
        [NonAction]
        private void CheckChangeShiffAndHalfDayOff(
            DateTime fromDate, 
            IList<WorkingCalendarsViewModel> viewModel,
            IList<UserChangedShift> userChangeShift,
            IList<HalfDayOff> halfDayOff)
        {
            foreach (UserChangedShift user in userChangeShift)
            {
                foreach (WorkingCalendarsViewModel employee in viewModel)
                {
                    if (employee.EmpId == user.EmpId)
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

            foreach (HalfDayOff item in halfDayOff)
            {
                foreach (WorkingCalendarsViewModel employee in viewModel)
                {
                    if (employee.EmpId == item.ID)
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
        }
    }
}
