namespace Fanex.NexHR.Controllers
{
    using System;
    using System.Collections.Generic;
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
    using Fanex.NexHR.Models.ScheduleArrange;
    using Fanex.NexHR.Models.Supervisor;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.ViewModels;
    using Fanex.NexHR.Web.Providers;

    public class ScheduleArrangeController : BaseController
    {
        private readonly IScheduleArrangeService scheduleArrangeService;
        private readonly ITeamService teamService;
        private readonly IEmployeeService employeeService;
        private readonly IBaseService baseService;

        public ScheduleArrangeController(
            IScheduleArrangeService scheduleArrangeService,
            ITeamService teamService,
            IEmployeeService employeeService,
            IBaseService baseService)
        {
            this.scheduleArrangeService = scheduleArrangeService;
            this.teamService = teamService;
            this.employeeService = employeeService;
            this.baseService = baseService;
        }

        [HRAuthorize(Policy.Read, FunctionKey.ScheduleArrange)]
        public CustomJsonResult GetFilter()
        {
            IList<Team> teams = teamService.GetAll(new GetAllGroupCriteria());

            var model = new ScheduleArrangeFilterViewModel
            {
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
        public CustomJsonResult GetWeeklySchedule(WeeklyScheduleFilter filter)
        {
            filter.SupervisorId = CurrentUser.Id;
            IEnumerable<WeeklySchedule> weeklySchedules = scheduleArrangeService.GetWeeklySchedule(filter);

            return new CustomJsonResult { Data = weeklySchedules, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
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

        [HttpPost]
        [HRAuthorize(Policy.Host, FunctionKey.ScheduleArrange)]
        public ActionResult EditRemark(int employeeId, string remark)
        {
            CommandResult commandResult = scheduleArrangeService.EditRemark(CurrentUser.Id, employeeId, remark);

            return new CustomJsonResult { Data = commandResult, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
    }
}