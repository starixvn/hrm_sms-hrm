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
    using System.Linq;
    using System.Web.Mvc;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interface;
    using Fanex.NexHR.ViewModels;

    /// <summary>
    /// Calendar Controller.
    /// </summary>
    public class CalendarController : BaseController
    {
        private readonly IWorkingCalendarService _workingCalendarService;
        private readonly ITeamService _teamService;
        private readonly IRequestService _requestService;
        private readonly IShiftService _shiftService;
        private readonly ILookupDataService _lookupDataService;
        private readonly IEmployeeService _employeeService;

        public CalendarController()
            : this(null, null, null, null, null, null)
        {
        }

        public CalendarController(
            IWorkingCalendarService workingCalendarService, 
            ITeamService teamService, 
            IRequestService requestService,
            IShiftService shiftService,
            ILookupDataService lookupDataService = null,
            IEmployeeService employeeService = null)
        {
            _workingCalendarService = workingCalendarService ?? new WorkingCalendarService();
            _teamService = teamService ?? new TeamService();
            _requestService = requestService ?? new RequestService();
            _shiftService = shiftService ?? new ShiftService();
            _lookupDataService = lookupDataService ?? new LookupDataService();
            _employeeService = employeeService ?? new EmployeeService();
        }

        /// <summary>
        /// Action Arrange Schedule.
        /// </summary>
        /// <returns>Action Result.</returns>
        [HttpGet]
        [Authorize(Roles = "ShiftManager")]
        public ActionResult ArrangeSchedule()
        {
            var currentDate = DateTime.Now;
            var fromDate = GetStartOfWeek(currentDate);
            var toDate = GetEndOfWeek(currentDate);
            var teams = _teamService.GetAll();
            var jobTitles = _lookupDataService.GetAll(LookupCodeName.JobTitle);
            var scheduleArrangeViewModel = new ScheduleArrangeFilterViewModel
            {
                FromDate = fromDate,
                ToDate = toDate,
                Groups = teams,
                JobTitles = jobTitles
            };

            return View(scheduleArrangeViewModel);
        }

        /// <summary>
        /// Post Action Arrange Schedule.
        /// </summary>
        /// <param name="scheduleArranges">Schedule Arrange.</param>
        /// <param name="shiftMultiples">Shift Multiple.</param>
        /// <returns>Action Result.</returns>
        [HttpPost]
        [Authorize(Roles = "ShiftManager")]
        public ActionResult ArrangeSchedule(string scheduleArranges, string shiftMultiples)
        {
            var errorCode = 0;
            var message = string.Empty;
            
            var scheduleArrangeDetail = JsonConverter.JsonExtensionMethod.FromJsonArray<ScheduleModel>(scheduleArranges);
            var shiftMultipleDetail = JsonConverter.JsonExtensionMethod.FromJsonArray<ShiftMultipleModel>(shiftMultiples);
            var scheduleArrangeModel = new ScheduleArrangeModel
            {
                ScheduleModels = scheduleArrangeDetail,
                ShiftMultipleModels = shiftMultipleDetail,
                ModifiedBy = this.CurrentUser.Id,
                ModifiedDate = DateTime.Now,
            };

            _workingCalendarService.InsertOrUpdateWorkingCalendars(scheduleArrangeModel, out errorCode, out message);

            return Json(new { errorCode = errorCode, message = message }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Action for load arrange schedule detail.
        /// </summary>
        /// <param name="fromDate">From Date.</param>
        /// <param name="toDate">The To Date.</param>
        /// <param name="groupIds">Groups id string.</param>
        /// <param name="jobTitles">Job titles.</param>
        /// <param name="employeeId">Employee Id.</param>
        /// <returns>Action Result.</returns>
        [HttpGet]
        [Authorize(Roles = "ShiftManager")]
        public ActionResult ArrangeScheduleDetail(DateTime fromDate, DateTime toDate, string groupIds, string jobTitles, int? employeeId = null)
        {
            var workingCalendars = _workingCalendarService.GetWorkingCalendarsForShiftArrange(this.CurrentUser.Id, fromDate, toDate, groupIds, jobTitles, employeeId);
            var scheduleArranges = ConvertWorkingCalendarToScheduleModel(fromDate, toDate, workingCalendars);

            var scheduleViewModel = new ScheduleViewModel
            {
                FromDate = fromDate,
                ToDate = toDate,
                ScheduleArranges = scheduleArranges
            };

            return View(scheduleViewModel);
        }

        /// <summary>
        /// Action to list of all shift in date range.
        /// </summary>
        /// <returns>Action Result.</returns>
        [Authorize(Roles = "ShiftManager,HR")]
        public ActionResult ShiftStatistic()
        {
            var currentDate = DateTime.Now;
            var fromDate = GetStartOfWeek(currentDate);
            var toDate = GetEndOfWeek(currentDate);
            var teams = _teamService.GetAll();

            var shiftStatisticViewModel = new ShiftStatisticViewModel
            {
                FromDate = fromDate,
                ToDate = toDate,
                Teams = teams
            };

            return View(shiftStatisticViewModel);
        }

        /// <summary>
        /// Action to get list shift count in date range.
        /// </summary>
        /// <param name="fromDate">From date.</param>
        /// <param name="toDate">The To date.</param>
        /// <param name="groupIds">Groups Id.</param>
        /// <param name="isShowWholeCompany">Is Show Whole Company.</param>
        /// <returns>Action Result.</returns>
        [Authorize(Roles = "ShiftManager,HR")]
        public ActionResult ShiftStatisticContent(DateTime fromDate, DateTime toDate, string groupIds, bool isShowWholeCompany)
        {
            var shiftStatisticModels = _workingCalendarService.GetShiftStatistic(this.CurrentUser.Id, fromDate, toDate, groupIds, isShowWholeCompany);

            var shiftStatisticContentViewModel = new ShiftStatisticContentViewModel
            {
                FromDate = fromDate, 
                ToDate = toDate,
                ShiftStatisticModels = shiftStatisticModels
            };

            return View(shiftStatisticContentViewModel);
        }

        public ActionResult GetEmployeesByShiftMonitor(string query)
        {
            var employees = _requestService.GetSupervisorByQuery(this.CurrentUser.Id, (int)RolesEnum.ShiftManager, null, query.Trim(), includeLoginUser: false);

            return Json(employees, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetShift(string query)
        {
            var shifts = _shiftService.GetByQuery(query.Trim());

            return Json(shifts, JsonRequestBehavior.AllowGet);
        }

        public ActionResult MyWorkingSchedule()
        {
            return View(_workingCalendarService.GetWorkingSchedule(DateTime.Now, this.CurrentUser.Id));
        }

        public ActionResult GetWorkingSchedule(DateTime date, int id)
        {
            var workingSchedule = _workingCalendarService.GetWorkingSchedule(date, id);
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

            ViewBag.Groups = _teamService.GetAll();
            var listJobTitle = _lookupDataService.GetAll(LookupCodeName.JobTitle);
            ViewBag.JobTitles = listJobTitle.Select(j => new SelectListItem
            {
                Text = j.Name,
                Value = j.Id.ToString(),
                Selected = j.Id.ToString() == string.Empty
            }).ToList();

            return View();
        }

        /// <summary>
        /// Workings the calendar details.
        /// </summary>
        /// <param name="empId">The employee identifier.</param>
        /// <param name="fromDate">From date.</param>
        /// <param name="toDate">Query To date.</param>
        /// <param name="groupId">The group identifier.</param>
        /// <param name="jobTitleId">The job title identifier.</param>
        /// <returns>ActionResult Return Partial view Working calendar Details.</returns>
        public ActionResult WorkingCalendarDetails(int? empId, DateTime fromDate, DateTime toDate, string groupId, string jobTitleId)
        {
            int month = 0;
            int year = 0;
            var firstDateInMonth = new System.Text.StringBuilder();
            ViewBag.Year = year != 0 ? year : DateTime.Now.Year;
            ViewBag.Month = month != 0 ? month : DateTime.Now.Month;
            firstDateInMonth.AppendFormat("{0}/{1}/1", ViewBag.Year, ViewBag.Month);
            var userChangedShift = _employeeService.GetUserChangedShift(new DateTime(ViewBag.Year, ViewBag.Month, 1));
            var userHalfDayOffs = _employeeService.GetHalfDayOffs(DateTime.Parse(firstDateInMonth.ToString())); 
           
            ViewBag.FromNewDate = fromDate;
            ViewBag.ToNewDate = toDate;
            var viewModel = _workingCalendarService.GetWorkingCalendars(null, fromDate, toDate, groupId, jobTitleId);
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
            var shiftManagerModel = _employeeService.GetEmployeeSupervisor(this.CurrentUser.Id, (short)RolesEnum.ShiftManager);
            var model = new ShiftManagerViewModel
            {
                ShiftManagerList = shiftManagerModel,
                EmployeeId = this.CurrentUser.Id,
                SupervisorRole = (short)RolesEnum.ShiftManager
            };

            return View(model);
        }

        /// <summary>
        /// Action to order schedule member.
        /// </summary>
        /// <param name="employeeIds">List Employee Ids.</param>
        /// <returns>Boolean to check.</returns>
        [HttpPost]
        public ActionResult MemberScheduleInOrder(string employeeIds)
        {
            var shiftArrange = new ShiftManagerViewModel
            {
                EmployeeId = this.CurrentUser.Id,
                SupervisorRole = (short)RolesEnum.ShiftManager,
                RoleShiftArrange = employeeIds
            };
            var result = _employeeService.InsertEmployeeShiftManager(shiftArrange, this.CurrentUser.Id);

            return Json(new { result = result == 0 }, JsonRequestBehavior.AllowGet);
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
