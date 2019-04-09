namespace Fanex.NexHR.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Constants;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Core.Models;
    using Fanex.NexHR.Core.Services;
    using Fanex.NexHR.Criteria.Calendar;
    using Fanex.NexHR.Criteria.Supervisor;
    using Fanex.NexHR.Criteria.Team;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Supervisor;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.ViewModels;
    using Fanex.NexHR.Web.Providers;

    public class ScheduleArrangeController : BaseController
    {
        private readonly IWorkingCalendarService workingCalendarService;
        private readonly ITeamService teamService;
        private readonly IEmployeeService employeeService;
        private readonly IBaseService baseService;

        public ScheduleArrangeController(
            IWorkingCalendarService workingCalendarService,
            ITeamService teamService,
            IEmployeeService employeeService,
            IBaseService baseService)
        {
            this.workingCalendarService = workingCalendarService;
            this.teamService = teamService;
            this.employeeService = employeeService;
            this.baseService = baseService;
        }

        [HRAuthorize(Policy.Read, FunctionKey.ScheduleArrange)]
        public CustomJsonResult GetFilter()
        {
            var currentDate = DateTime.Now;
            DateTime fromDate = GetStartOfWeek(currentDate);
            DateTime toDate = GetEndOfWeek(currentDate);
            IList<Team> teams = teamService.GetAll(new GetAllGroupCriteria());

            var model = new ScheduleArrangeFilterViewModel
            {
                FromDate = fromDate,
                ToDate = toDate,
                Groups = teams
            };

            return new CustomJsonResult { Data = model, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public CustomJsonResult GetEmployeesByShiftMonitor(string query)
        {
            var criteria = new GetEmployeeSupervisorByQueryCriteria
            {
                EmpId = CurrentUser.Id,
                Role = (int)RoleSystem.ShiftManager,
                RequestType = null,
                Query = query.Trim(),
                IncludeLoginUser = false
            };
            IList<SupervisorNameModel> employees = employeeService.GetSupervisorByQuery(criteria);

            return new CustomJsonResult { Data = employees, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HRAuthorize(Policy.Read, FunctionKey.ScheduleArrange)]
        public CustomJsonResult ScheduleArrangeDetails(DateTime fromDate, DateTime toDate, string groupIds, string jobTitles, int? employeeId = null)
        {
            var criteria = new GetWorkingCalendarsForShiftArrangeCriteria
            {
                SupervisorId = CurrentUser.Id,
                Fromtime = fromDate,
                Totime = toDate,
                GroupIds = groupIds,
                JobTitleId = jobTitles,
                EmpId = employeeId
            };
            IList<WorkingCalendarModel> workingCalendars = workingCalendarService.GetWorkingCalendarsForShiftArrange(criteria);
            var scheduleArranges = new List<ScheduleArrangeViewModel>();

            if (workingCalendars.Any())
            {
                scheduleArranges = ConvertWorkingCalendarToScheduleModel(fromDate, toDate, workingCalendars).ToList();
            }

            return new CustomJsonResult { Data = scheduleArranges, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HttpPost]
        [HRAuthorize(Policy.Host, FunctionKey.ScheduleArrange)]
        public ActionResult Edit(IList<ScheduleModel> scheduleArranges, IList<ShiftMultipleModel> shiftMultiples)
        {
            var scheduleArrangeModel = new ScheduleArrangeModel
            {
                ScheduleModels = scheduleArranges,
                ShiftMultipleModels = shiftMultiples,
                ModifiedBy = CurrentUser.Id,
                ModifiedDate = DateTime.Now,
            };

            var criteria = new InsertOrUpdateWorkingCalendarsCriteria
            {
                ModifiedDate = scheduleArrangeModel.ModifiedDate,
                ModifiedBy = scheduleArrangeModel.ModifiedBy,
                ScheduleList = scheduleArrangeModel.ScheduleModelXML,
                ShiftMultipleList = scheduleArrangeModel.ShiftMultipleXML
            };

            CommandResult commandResult = baseService.ExecuteNonQuery(criteria);

            return new CustomJsonResult { Data = commandResult, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
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

        private IList<ScheduleArrangeViewModel> ConvertWorkingCalendarToScheduleModel(
            DateTime fromDate,
            DateTime toDate,
            IList<WorkingCalendarModel> workingCalendars)
        {
            var scheduleModels = new List<ScheduleArrangeViewModel>();
            var employees = workingCalendars.GroupBy(x => x.Employee.Id).Select(x => x.Key);

            foreach (var employeeId in employees)
            {
                var scheduleArrangeModel = HandleConvertWorkingCalendarToScheduleModel(employeeId, fromDate, toDate, workingCalendars);
                scheduleModels.Add(scheduleArrangeModel);
            }

            return scheduleModels;
        }

        private ScheduleArrangeViewModel HandleConvertWorkingCalendarToScheduleModel(
            int employeeId,
            DateTime fromDate,
            DateTime toDate,
            IList<WorkingCalendarModel> workingCalendars)
        {
            var numberDays = (toDate - fromDate).Days + 1;
            IEnumerable<WorkingCalendarModel> employeeWorkingCalendars = null;
            WorkingCalendarModel workingCalendar = null;
            ScheduleArrangeViewModel scheduleArrangeModel = null;
            var workingDate = DateTime.Now.Date;
            var scheduleHasInfo = false;
            for (int i = 0; i < numberDays; i++)
            {
                workingDate = fromDate.AddDays(i);
                employeeWorkingCalendars = workingCalendars.Where(
                    t => (t.WorkDate.Date == workingDate && t.Employee.Id == employeeId));

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

            return scheduleArrangeModel;
        }

        private static void SetWorkingCalendarForDayOfWeek(
            ScheduleArrangeViewModel scheduleModel,
            WorkingCalendarModel workingCalendar)
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

                default:
                    // nothing
                    break;
            }
        }
    }
}