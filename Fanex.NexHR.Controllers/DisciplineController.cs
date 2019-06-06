namespace Fanex.NexHR.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Core;
    using Core.Models;
    using Core.Services;
    using Fanex.NexHR.Core.Constants;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Core.Filters;
    using Fanex.NexHR.Criteria.Discipline;
    using Fanex.NexHR.Criteria.Team;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Discipline;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.ViewModels.Discipline;

    public class DisciplineController : BaseController
    {
        private readonly ITeamService teamService;
        private readonly IDisciplineService disciplineService;
        private readonly IBaseService baseService;

        public DisciplineController(
            ISessionService sessionService,
            ITeamService teamService,
            IDisciplineService disciplineService,
            IBaseService baseService) : base(sessionService)
        {
            this.teamService = teamService;
            this.disciplineService = disciplineService;
            this.baseService = baseService;
        }

        [HRMPermission(Policy.Read, FunctionKey.Discipline)]
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

            ViewBag.IsHost = sessionService.HasPermissionOnFunction(FunctionKey.Discipline, Policy.Host);
            ViewBag.IsHostOtherDiscipline = sessionService.HasPermissionOnFunction(FunctionKey.OtherDiscipline, Policy.Host);
            return View(model);
        }

        [HRMPermission(Policy.Read, FunctionKey.Discipline)]
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
                ApproverId = sessionService.Employee.Id,
                IsShowAll = model.IsShowAll
            };

            IList<DisciplineSummaryModel> disciplines = disciplineService.GetDisciplineSummary(criteria);

            ViewBag.IsHost = sessionService.HasPermissionOnFunction(FunctionKey.Discipline, Policy.Host);

            return PartialView("_ListDisciplineSummary", disciplines);
        }

        [HRMPermission(Policy.Read, FunctionKey.Discipline)]
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
            DateTime fromDate = (month >= 1 && month <= 6) ? new DateTime(today.Year, 1, 1) : new DateTime(today.Year, 7, 1);
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

        [HRMPermission(Policy.Read, FunctionKey.Discipline)]
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
                ApproverId = sessionService.Employee.Id
            };

            IList<DisciplineDetailModel> disciplines = disciplineService.GetDisciplineDetail(criteria);
            ViewBag.HasPermissionToEdit = sessionService.HasPermissionOnFunction(FunctionKey.Discipline, Policy.Host);
            ViewBag.HasPermissionToUpdateOutOfControlReason = sessionService.HasPermissionOnFunction(FunctionKey.UpdateOutOfControlReason, Policy.Host);

            return PartialView("_ListDisciplineDetail", disciplines);
        }

        [HRMPermission(Policy.Read, FunctionKey.Discipline)]
        public ActionResult ViewOtherDiscipline(int employeeId, DateTime fromDate, DateTime toDate)
        {
            var criteria = new GetOtherDisciplineOfEmployeeCriteria
            {
                EmployeeId = employeeId,
                FromDate = fromDate,
                ToDate = toDate
            };

            IList<OtherDisciplineModel> otherDisciplines = disciplineService.GetOtherDiscipline(criteria);
            ViewBag.IsHost = sessionService.HasPermissionOnFunction(FunctionKey.Discipline, Policy.Host);

            return PartialView("_ViewOtherDiscipline", otherDisciplines);
        }

        [HRMPermission(Policy.Read, FunctionKey.OtherDiscipline)]
        public ActionResult ViewWeekendLeave(int employeeId, DateTime fromDate, DateTime toDate)
        {
            var criteria = new GetWeekendLeaveOfEmployeeCriteria
            {
                EmployeeId = employeeId,
                FromDate = fromDate,
                ToDate = toDate
            };

            IList<OtherDisciplineModel> weekendLeaves = disciplineService.GetWeekendLeave(criteria);
            ViewBag.IsHost = sessionService.HasPermissionOnFunction(FunctionKey.Discipline, Policy.Host);

            return PartialView("_ViewWeekendLeave", weekendLeaves);
        }

        [HRMPermission(Policy.Host, FunctionKey.OtherDiscipline)]
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

        [HRMPermission(Policy.Read, FunctionKey.OtherDiscipline)]
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
        [HRMPermission(Policy.Host, FunctionKey.OtherDiscipline)]
        public JsonResult AddOtherDiscipline(AddOtherDisciplineViewModel model)
        {
            var criteria = new InsertOtherDisciplineCriteria
            {
                EmployeeId = model.EmployeeId,
                WorkDate = model.WorkDate,
                DisciplineTypeId = model.DisciplineTypeId,
                Score = model.MinusScore,
                Comment = model.Comment,
                CreatedBy = sessionService.Employee.Id
            };
            CommandResult commandResult = baseService.ExecuteNonQuery(criteria);

            return Json(commandResult);
        }

        [HttpPost]
        [ValidateInput(false)]
        [HRMPermission(Policy.Host, FunctionKey.OtherDiscipline)]
        public JsonResult AddWeekendLeave(AddOtherDisciplineViewModel model)
        {
            var criteria = new InsertWeekendLeaveCriteria
            {
                EmployeeId = model.EmployeeId,
                WorkDate = model.WorkDate,
                Score = model.MinusScore,
                CreatedBy = sessionService.Employee.Id
            };
            CommandResult commandResult = baseService.ExecuteNonQuery(criteria);

            return Json(commandResult);
        }

        [HttpPost]
        [HRMPermission(Policy.Host, FunctionKey.OtherDiscipline)]
        public string DeleteOtherDiscipline(DeleteOtherDisciplineCriteria criteria)
        {
            if (criteria != null)
            {
                criteria.ModifiedBy = sessionService.Employee.Id;
                CommandResult commandResult = baseService.ExecuteNonQuery(criteria);

                if (!commandResult.IsSuccessful)
                {
                    return commandResult.Message;
                }
            }

            return string.Empty;
        }

        [HttpPost]
        [HRMPermission(Policy.Host, FunctionKey.OtherDiscipline)]
        public string DeleteWeekendLeave(DeleteWeekendLeaveCriteria criteria)
        {
            if (criteria != null)
            {
                criteria.ModifiedBy = sessionService.Employee.Id;
                CommandResult commandResult = baseService.ExecuteNonQuery(criteria);

                if (!commandResult.IsSuccessful)
                {
                    return commandResult.Message;
                }
            }

            return string.Empty;
        }

        [HttpPost]
        [HRMPermission(Policy.Host, FunctionKey.Discipline)]
        public JsonResult UpdateIgnoredDiscipline(IList<IgnoredDisciplineModel> ignoredDisciplineDetails)
        {
            var criteria = new UpdateIgnoredDisciplineCriteria
            {
                IgnoredDisciplines = ignoredDisciplineDetails,
                HasUpdateOutOfControlReason = sessionService.HasPermissionOnFunction(FunctionKey.UpdateOutOfControlReason, Policy.Host),
                ModifiedBy = sessionService.Employee.Id,
            };

            CommandResult commandResult = baseService.ExecuteNonQuery(criteria);

            return Json(commandResult, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetMemberToAddOtherDiscipline(string keyword)
        {
            var criteria = new GetMemberToAddOtherDiscipineBySupervisorIdCriteria
            {
                SupervisorId = sessionService.Employee.Id,
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