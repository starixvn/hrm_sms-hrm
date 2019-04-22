namespace Fanex.NexHR.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Constants;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Core.Models;
    using Fanex.NexHR.Criteria.Supervisor;
    using Fanex.NexHR.Criteria.Team;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.ScheduleArrange;
    using Fanex.NexHR.Models.Supervisor;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.ViewModels;
    using Fanex.NexHR.ViewModels.ScheduleArrange;
    using Fanex.NexHR.Web.Providers;

    public class ScheduleArrangeController : BaseController
    {
        private readonly IScheduleArrangeService scheduleArrangeService;
        private readonly ITeamService teamService;
        private readonly IEmployeeService employeeService;

        public ScheduleArrangeController(
            IScheduleArrangeService scheduleArrangeService,
            ITeamService teamService,
            IEmployeeService employeeService)
        {
            this.scheduleArrangeService = scheduleArrangeService;
            this.teamService = teamService;
            this.employeeService = employeeService;
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
            IEnumerable<WeeklyScheduleViewModel> groups = weeklySchedules.GroupBy(x => new { x.Team.Id, x.Team.Code, x.Team.Name })
                .Select(x => new WeeklyScheduleViewModel
                {
                    GroupId = x.Key.Id,
                    Name = x.Key.Name,
                    Code = x.Key.Code,
                    Members = x
                });

            return new CustomJsonResult { Data = groups, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HttpPost]
        [HRAuthorize(Policy.Host, FunctionKey.ScheduleArrange)]
        public ActionResult EditWeekly(IEnumerable<EditDailySchedule> weeklySchedules, IEnumerable<EditDefaultShift> defaultShifts)
        {
            CommandResult commandResult = scheduleArrangeService.EditWeekly(CurrentUser.Id, weeklySchedules, defaultShifts);

            return new CustomJsonResult { Data = commandResult, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HttpPost]
        [HRAuthorize(Policy.Host, FunctionKey.ScheduleArrange)]
        public ActionResult EditRemark(int employeeId, string remark)
        {
            CommandResult commandResult = scheduleArrangeService.EditRemark(CurrentUser.Id, employeeId, remark);

            return new CustomJsonResult { Data = commandResult, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HttpPost]
        [HRAuthorize(Policy.Host, FunctionKey.ScheduleArrange)]
        public ActionResult EditDisplayOrder(IEnumerable<EditDisplayOrder> displayOrders)
        {
            CommandResult commandResult = scheduleArrangeService.EditDisplayOrder(CurrentUser.Id, displayOrders);

            return new CustomJsonResult { Data = commandResult, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HttpPost]
        [HRAuthorize(Policy.Host, FunctionKey.ScheduleArrange)]
        public ActionResult EditGroupOrder(IEnumerable<EditGroupOrder> displayOrders)
        {
            CommandResult commandResult = scheduleArrangeService.EditGroupOrder(CurrentUser.Id, displayOrders);

            return new CustomJsonResult { Data = commandResult, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
    }
}