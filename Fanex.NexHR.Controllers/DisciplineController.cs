namespace Fanex.NexHR.Controllers
{
    using Core;
    using Fanex.NexHR.Criteria.Discipline;
    using Fanex.NexHR.Criteria.Team;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Discipline;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Implement;
    using Fanex.NexHR.Services.Interfaces;
    using Fanex.NexHR.ViewModels.Discipline;
    using Fanex.NexHR.Web.Providers;
    using Nexus.RoleManagement.Services;
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;

    public class DisciplineController : BaseController
    {
        private readonly ITeamService teamService;
        private readonly IDisciplineService disciplineService;
        private readonly ICommandService commandService;

        public DisciplineController()
            : this(null, null, null)
        {
        }

        public DisciplineController(
            ITeamService teamService,
            IDisciplineService disciplineService,
            ICommandService commandService)
        {
            this.teamService = teamService ?? new TeamService();
            this.disciplineService = disciplineService ?? new DisciplineService();
            this.commandService = commandService ?? new CommandService();
        }

        [HRAuthorize(Policy.Read, FunctionKey.Discipline)]
        public ActionResult DisciplineSummary(DisciplineSummaryViewModel model)
        {
            if (model != null)
            {
                model.FromDate = GetFromDate();
                model.ToDate = GetToDate();
                model.Teams = GetAllTeam();
                model.DisciplineTypes = GetDisciplineType(true);
                model.DateRange = Utilities.GetDateRangeType();
            }

            ViewBag.IsHost = CurrentUser.HasPermission(FunctionKey.Discipline, Policy.Host);

            return View(model);
        }

        [HRAuthorize(Policy.Read, FunctionKey.Discipline)]
        public ActionResult PartialListDisciplineSummary(DisciplineSummaryViewModel model)
        {
            var criteria = new GetDisciplineSummaryCriteria
            {
                FromDate = model.FromDate,
                ToDate = model.ToDate,
                GroupId = model.TeamId,
                IsLocal = model.IsLocal,
                EmployeeId = model.EmployeeId,
                ReasonId = model.ReasonId,
                ApproverId = CurrentUser.Id,
                IsShowAll = model.IsShowAll
            };

            IList<DisciplineSummaryModel> disciplines = disciplineService.GetDisciplineSummary(criteria);

            ViewBag.IsHost = CurrentUser.HasPermission(FunctionKey.Discipline, Policy.Host);

            return PartialView("_ListDisciplineSummary", disciplines);
        }

        [HRAuthorize(Policy.Read, FunctionKey.Discipline)]
        public ActionResult DisciplineDetail(DisciplineSummaryViewModel model)
        {
            if (model != null)
            {
                if (model.FromDate == default(DateTime))
                {
                    model.FromDate = GetFromDate();
                    model.ToDate = GetToDate();
                }

                if (model.ReasonId < 0)
                {
                    model.ReasonId = null;
                }

                model.Teams = GetAllTeam();
                model.DisciplineTypes = GetDisciplineType(false);
                model.DateRange = Utilities.GetDateRangeType();
            }

            return View(model);
        }

        private static DateTime GetFromDate()
        {
            var today = DateTime.Today;
            int month = DateTime.Today.Month;
            DateTime fromDate = (month >= 1 && month <= 6) ? new DateTime(today.Year, 1, 1) : new DateTime(today.Year, 6, 1);
            return fromDate;
        }

        private static DateTime GetToDate()
        {
            var today = DateTime.Today;
            int month = DateTime.Today.Month;
            DateTime toDate = (month >= 1 && month <= 6)
                    ? new DateTime(today.Year, 6, DateTime.DaysInMonth(today.Year, 6))
                    : new DateTime(today.Year, 12, DateTime.DaysInMonth(today.Year, 12));
            return toDate;
        }

        [HRAuthorize(Policy.Read, FunctionKey.Discipline)]
        public ActionResult PartialListDisciplineDetail(DisciplineSummaryViewModel model)
        {
            var criteria = new GetDisciplineDetailCriteria
            {
                FromDate = model.FromDate,
                ToDate = model.ToDate,
                GroupId = model.TeamId,
                IsLocal = model.IsLocal,
                EmployeeId = model.EmployeeId,
                ReasonId = model.ReasonId,
                HasIgnored = model.HasIgnored,
                ApproverId = CurrentUser.Id
            };

            IList<DisciplineDetailModel> disciplines = disciplineService.GetDisciplineDetail(criteria);
            ViewBag.HasPermissionToEdit = CurrentUser.HasPermission(FunctionKey.Discipline, Policy.Host);

            return PartialView("_ListDisciplineDetail", disciplines);
        }

        [HRAuthorize(Policy.Read, FunctionKey.Discipline)]
        public ActionResult ViewOtherDiscipline(int employeeId, DateTime fromDate, DateTime toDate)
        {
            var criteria = new GetOtherDisciplineOfEmployeeCriteria
            {
                EmployeeId = employeeId,
                FromDate = fromDate,
                ToDate = toDate
            };

            IList<OtherDisciplineModel> otherDisciplines = disciplineService.GetOtherDiscipline(criteria);
            ViewBag.IsHost = CurrentUser.HasPermission(FunctionKey.Discipline, Policy.Host);

            return PartialView("_ViewOtherDiscipline", otherDisciplines);
        }

        [HRAuthorize(Policy.Read, FunctionKey.Discipline)]
        public ActionResult ViewWeekendLeave(int employeeId, DateTime fromDate, DateTime toDate)
        {
            var criteria = new GetWeekendLeaveOfEmployeeCriteria
            {
                EmployeeId = employeeId,
                FromDate = fromDate,
                ToDate = toDate
            };

            IList<OtherDisciplineModel> weekendLeaves = disciplineService.GetWeekendLeave(criteria);
            ViewBag.IsHost = CurrentUser.HasPermission(FunctionKey.Discipline, Policy.Host);

            return PartialView("_ViewWeekendLeave", weekendLeaves);
        }

        [HRAuthorize(Policy.Read, FunctionKey.Discipline)]
        public ActionResult PartialAddOtherDiscipline(string employee, int? employeeId = null)
        {
            var criteria = new GetOtherDisciplineTypeCriteria();
            IList<DisciplineTypeModel> otherDisciplineTypes = disciplineService.GetOtherDisciplineType(criteria);

            var model = new AddOtherDisciplineViewModel
            {
                EmployeeId = employeeId ?? -1,
                Employee = employee,
                WorkDate = DateTime.Now,
                OtherDisciplineTypes = otherDisciplineTypes
            };

            return PartialView("_AddOtherDiscipline", model);
        }

        [HRAuthorize(Policy.Read, FunctionKey.Discipline)]
        public ActionResult PartialAddWeekendLeave()
        {
            var model = new AddOtherDisciplineViewModel
            {
                EmployeeId = -1,
                MinusScore = 3.5M,
                WorkDate = DateTime.Now
            };

            return PartialView("_AddWeekendLeave", model);
        }

        [HttpPost]
        [ValidateInput(false)]
        [HRAuthorize(Policy.Host, FunctionKey.Discipline)]
        public JsonResult AddOtherDiscipline(AddOtherDisciplineViewModel model)
        {
            var criteria = new InsertOtherDisciplineCriteria
            {
                EmployeeId = model.EmployeeId,
                WorkDate = model.WorkDate,
                DisciplineTypeId = model.DisciplineTypeId,
                Score = model.MinusScore,
                Comment = model.Comment,
                CreatedBy = CurrentUser.Id
            };

            CommandResult commandResult = commandService.Insert(criteria);

            return Json(commandResult);
        }

        [HttpPost]
        [ValidateInput(false)]
        [HRAuthorize(Policy.Host, FunctionKey.Discipline)]
        public JsonResult AddWeekendLeave(AddOtherDisciplineViewModel model)
        {
            var criteria = new InsertWeekendLeaveCriteria
            {
                EmployeeId = model.EmployeeId,
                WorkDate = model.WorkDate,
                Score = model.MinusScore,
                CreatedBy = CurrentUser.Id
            };

            CommandResult commandResult = commandService.Insert(criteria);

            return Json(commandResult);
        }

        [HttpPost]
        [HRAuthorize(Policy.Host, FunctionKey.Discipline)]
        public string DeleteOtherDiscipline(DeleteOtherDisciplineCriteria criteria)
        {
            if (criteria != null)
            {
                criteria.ModifiedBy = CurrentUser.Id;
                CommandResult commandResult = commandService.Delete(criteria);

                if (!commandResult.IsSuccessful)
                {
                    return commandResult.Message;
                }
            }

            return string.Empty;
        }

        [HttpPost]
        [HRAuthorize(Policy.Host, FunctionKey.Discipline)]
        public string DeleteWeekendLeave(DeleteWeekendLeaveCriteria criteria)
        {
            if (criteria != null)
            {
                criteria.ModifiedBy = CurrentUser.Id;
                CommandResult commandResult = commandService.Delete(criteria);

                if (!commandResult.IsSuccessful)
                {
                    return commandResult.Message;
                }
            }

            return string.Empty;
        }

        [HttpPost]
        [HRAuthorize(Policy.Host, FunctionKey.Discipline)]
        public JsonResult UpdateIgnoredDiscipline(IList<IgnoredDisciplineModel> ignoredDisciplineDetails)
        {
            var criteria = new UpdateIgnoredDisciplineCriteria
            {
                IgnoredDisciplines = ignoredDisciplineDetails,
                ModifiedBy = CurrentUser.Id,
            };

            CommandResult commandResult = commandService.Update(criteria);

            return Json(commandResult, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetMemberToAddOtherDiscipline(string keyword)
        {
            var criteria = new GetMemberToAddOtherDiscipineBySupervisorIdCriteria
            {
                SupervisorId = CurrentUser.Id,
                Query = keyword
            };

            IList<Employee> employees = disciplineService.GetMemberToAddOtherDiscipline(criteria);

            return Json(employees, JsonRequestBehavior.AllowGet);
        }

        private IList<Team> GetAllTeam()
        {
            IList<Team> teamList = teamService.GetAll(new GetAllGroupCriteria());

            return teamList;
        }

        private IList<DisciplineTypeModel> GetDisciplineType(bool includeOtherType)
        {
            var criteria = new GetDisciplineTypeCriteria
            {
                IncludeOtherType = includeOtherType
            };
            IList<DisciplineTypeModel> disciplineTypes = disciplineService.GetDisciplineType(criteria);

            return disciplineTypes;
        }
    }
}