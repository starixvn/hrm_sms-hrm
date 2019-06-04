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
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.ScheduleArrange;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.ViewModels.ScheduleArrange;
    using Fanex.NexHR.Web.Providers;

    public class ScheduleArrangeController : BaseController
    {
        private const string DateFormat = "dd-MMM-yyyy";
        private readonly IScheduleArrangeService scheduleArrangeService;
        private readonly IExportExcelScheduleService exportExcelService;

        public ScheduleArrangeController(
            IScheduleArrangeService scheduleArrangeService,
            IExportExcelScheduleService exportExcelService)
        {
            this.scheduleArrangeService = scheduleArrangeService;
            this.exportExcelService = exportExcelService;
        }

        [HttpGet]
        [HRAuthorize(Policy.Host, FunctionKey.ScheduleArrange)]
        public ActionResult Index()
        {
            return View();
        }

        public CustomJsonResult GetGroups()
        {
            IEnumerable<EntityBase<int>> groups = scheduleArrangeService.GetGroups(CurrentUser.Id);

            return new CustomJsonResult { Data = groups, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public CustomJsonResult GetEmployees(string groupIds, string query)
        {
            IEnumerable<EmployeeSuggestion> employees = scheduleArrangeService.GetEmployees(CurrentUser.Id, groupIds, query);

            return new CustomJsonResult { Data = employees, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HRAuthorize(Policy.Host, FunctionKey.ScheduleArrange)]
        public CustomJsonResult GetWeeklySchedule(WeeklyScheduleFilter filter)
        {
            filter.ActorId = CurrentUser.Id;
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
        [HRAuthorize(Policy.Host, FunctionKey.ScheduleArrange)]
        public ActionResult EditWeekly(
            DateTime fromDate,
            DateTime toDate,
            IEnumerable<EditDailySchedule> weeklySchedules,
            IEnumerable<EditDefaultShift> defaultShifts)
        {
            CommandResult commandResult = scheduleArrangeService.EditWeekly(
                CurrentUser.Id,
                fromDate,
                toDate,
                weeklySchedules,
                defaultShifts);

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

        [HttpPost]
        [HRAuthorize(Policy.Host, FunctionKey.ScheduleArrange)]
        public ActionResult EditWeeklyDefaultShift(WeeklyDefaultShift weeklyDefaultShift, bool isUsedWeeklyDefaultShift)
        {
            CommandResult commandResult = scheduleArrangeService.EditWeeklyDefaultShift(CurrentUser.Id, weeklyDefaultShift, isUsedWeeklyDefaultShift);

            return new CustomJsonResult { Data = commandResult, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HttpGet]
        [HRAuthorize(Policy.Host, FunctionKey.ScheduleArrange)]
        public ActionResult ExportExcel(WeeklyScheduleFilter filter)
        {
            filter.ActorId = CurrentUser.Id;
            IEnumerable<WeeklySchedule> weeklySchedules = scheduleArrangeService.GetWeeklySchedule(filter);
            byte[] bytes = exportExcelService.Export(filter, weeklySchedules);

            var fileName = $"Working shift {filter.FromDate.ToString(DateFormat)} to {filter.ToDate.ToString(DateFormat)}.xlsx";
            return File(bytes, "application/xlsx", fileName);
        }

        [HttpGet]
        [HRAuthorize(Policy.Host, FunctionKey.ScheduleArrange)]
        public ActionResult ExportExcelDriverPickup(WeeklyScheduleFilter filter)
        {
            Actor actor = new Actor
            {
                ActorId = CurrentUser.Id,
                Nickname = CurrentUser.NickName,
                FullName = CurrentUser.FullName
            };
            filter.ActorId = CurrentUser.Id;
            IEnumerable<WeeklySchedule> weeklySchedules = scheduleArrangeService.GetWeeklySchedule(filter);
            byte[] bytes = exportExcelService.ExportDriverPickup(actor, filter, weeklySchedules);

            var fileName = $"Schedule for Driver {filter.FromDate.ToString(DateFormat)} to {filter.ToDate.ToString(DateFormat)}.xlsx";
            return File(bytes, "application/xlsx", fileName);
        }

        private bool CheckPermission(DateTime fromDate)
        {
            bool isHost = false;
            DateTime firstDayOfWeek = DateTime.Now.FirstDayOfWeek();

            if (CurrentUser.HasPermission(FunctionKey.EditScheduleUnlimited, Policy.Host))
            {
                isHost = true;
            }
            else if (CurrentUser.HasPermission(FunctionKey.EditScheduleFrom4WeeksAgo, Policy.Host))
            {
                const int FourWeeksAgo = 28;
                isHost = fromDate.Date >= firstDayOfWeek.AddDays(-FourWeeksAgo);
            }
            else if (CurrentUser.HasPermission(FunctionKey.EditScheduleFromNextWeek, Policy.Host))
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