namespace Fanex.NexHR.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Constants;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Core.Filters;
    using Fanex.NexHR.Core.Models;
    using Fanex.NexHR.Core.Services;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.ScheduleArrange;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.ViewModels.ScheduleArrange;

    public class ScheduleArrangeController : BaseController
    {
        private const string DateFormat = "dd-MMM-yyyy";
        private readonly IScheduleArrangeService scheduleArrangeService;
        private readonly IExportExcelScheduleService exportExcelService;

        public ScheduleArrangeController(
            ISessionService sessionService,
            IScheduleArrangeService scheduleArrangeService,
            IExportExcelScheduleService exportExcelService) : base(sessionService)
        {
            this.scheduleArrangeService = scheduleArrangeService;
            this.exportExcelService = exportExcelService;
        }

        [HttpGet]
        [HRMPermission(Policy.Host, FunctionKey.ScheduleArrange)]
        public ActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult IFrame()
        {
            return View();
        }

        [AllowAnonymous]
        public CustomJsonResult GetGroups(bool isIFrame = false)
        {
            IEnumerable<EntityBase<int>> groups = scheduleArrangeService.GetGroups(sessionService.Employee.Id, isIFrame);

            return new CustomJsonResult { Data = groups, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [AllowAnonymous]
        public CustomJsonResult GetEmployees(GetEmployeeFilter filter)
        {
            filter.ActorId = sessionService.Employee.Id;
            IEnumerable<EmployeeSuggestion> employees = scheduleArrangeService.GetEmployees(filter);

            return new CustomJsonResult { Data = employees, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [AllowAnonymous]
        public CustomJsonResult GetWeeklySchedule(WeeklyScheduleFilter filter)
        {
            filter.ActorId = sessionService.Employee.Id;
            IEnumerable<WeeklySchedule> weeklySchedules = scheduleArrangeService.GetWeeklySchedule(filter);
            IEnumerable<WeeklyScheduleViewModel> schedules = weeklySchedules.GroupBy(x => new { x.Team.Id, x.Team.Code, x.Team.Name })
                .Select(x => new WeeklyScheduleViewModel
                {
                    GroupId = x.Key.Id,
                    Name = x.Key.Name,
                    Code = x.Key.Code,
                    Members = x
                });

            bool isHost = CheckPermission(filter.FromDate);
            var data = new { isHost, schedules };

            return new CustomJsonResult { Data = data, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HttpPost]
        [HRMPermission(Policy.Host, FunctionKey.ScheduleArrange)]
        public ActionResult EditWeekly(
            DateTime fromDate,
            DateTime toDate,
            IEnumerable<EditDailySchedule> weeklySchedules,
            IEnumerable<EditDefaultShift> defaultShifts)
        {
            CommandResult commandResult = scheduleArrangeService.EditWeekly(
                sessionService.Employee.Id,
                fromDate,
                toDate,
                weeklySchedules,
                defaultShifts);

            return new CustomJsonResult { Data = commandResult, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HttpPost]
        [HRMPermission(Policy.Host, FunctionKey.ScheduleArrange)]
        public ActionResult EditRemark(EditRemark model)
        {
            CommandResult commandResult = scheduleArrangeService.EditRemark(sessionService.Employee.Id, model);

            return new CustomJsonResult { Data = commandResult, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HttpPost]
        [HRMPermission(Policy.Host, FunctionKey.ScheduleArrange)]
        public ActionResult EditDisplayOrder(IEnumerable<EditDisplayOrder> displayOrders)
        {
            CommandResult commandResult = scheduleArrangeService.EditDisplayOrder(sessionService.Employee.Id, displayOrders);

            return new CustomJsonResult { Data = commandResult, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HttpPost]
        [HRMPermission(Policy.Host, FunctionKey.ScheduleArrange)]
        public ActionResult EditGroupOrder(IEnumerable<EditGroupOrder> displayOrders)
        {
            CommandResult commandResult = scheduleArrangeService.EditGroupOrder(sessionService.Employee.Id, displayOrders);

            return new CustomJsonResult { Data = commandResult, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HttpPost]
        [HRMPermission(Policy.Host, FunctionKey.ScheduleArrange)]
        public ActionResult EditWeeklyDefaultShift(WeeklyDefaultShift weeklyDefaultShift, bool isUsedWeeklyDefaultShift)
        {
            CommandResult commandResult = scheduleArrangeService.EditWeeklyDefaultShift(sessionService.Employee.Id, weeklyDefaultShift, isUsedWeeklyDefaultShift);

            return new CustomJsonResult { Data = commandResult, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HttpGet]
        [HRMPermission(Policy.Host, FunctionKey.ScheduleArrange)]
        public ActionResult ExportExcel(WeeklyScheduleFilter filter)
        {
            filter.ActorId = sessionService.Employee.Id;
            IEnumerable<WeeklySchedule> weeklySchedules = scheduleArrangeService.GetWeeklySchedule(filter);
            byte[] bytes = exportExcelService.Export(filter, weeklySchedules);

            var fileName = $"Working shift {filter.FromDate.ToString(DateFormat)} to {filter.ToDate.ToString(DateFormat)}.xlsx";
            return File(bytes, "application/xlsx", fileName);
        }

        [HttpGet]
        [HRMPermission(Policy.Host, FunctionKey.ScheduleArrange)]
        public ActionResult ExportExcelDriverPickup(WeeklyScheduleFilter filter)
        {
            Actor actor = new Actor
            {
                ActorId = sessionService.Employee.Id,
                Nickname = sessionService.Employee.Nickname,
                FullName = sessionService.Employee.FullName
            };
            filter.ActorId = sessionService.Employee.Id;
            IEnumerable<WeeklySchedule> weeklySchedules = scheduleArrangeService.GetWeeklySchedule(filter);
            byte[] bytes = exportExcelService.ExportDriverPickup(actor, filter, weeklySchedules);

            var fileName = $"Schedule for Driver {filter.FromDate.ToString(DateFormat)} to {filter.ToDate.ToString(DateFormat)}.xlsx";
            return File(bytes, "application/xlsx", fileName);
        }

        private bool CheckPermission(DateTime fromDate)
        {
            bool isHost = false;
            DateTime firstDayOfWeek = DateTime.Now.FirstDayOfWeek();

            if (sessionService.HasPermissionOnFunction(FunctionKey.EditScheduleUnlimited, Policy.Host))
            {
                isHost = true;
            }
            else if (sessionService.HasPermissionOnFunction(FunctionKey.EditScheduleFrom4WeeksAgo, Policy.Host))
            {
                const int FourWeeksAgo = 28;
                isHost = fromDate.Date >= firstDayOfWeek.AddDays(-FourWeeksAgo);
            }
            else if (sessionService.HasPermissionOnFunction(FunctionKey.EditScheduleFromNextWeek, Policy.Host))
            {
                const int NextWeek = 7;
                isHost = fromDate.Date >= firstDayOfWeek.AddDays(NextWeek);
            }
            else
            {
                isHost = false;
            }

            return isHost;
        }
    }
}