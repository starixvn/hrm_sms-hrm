namespace Fanex.NexHR.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using Fanex.NexHR.Core.Enums;
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

        public ActionResult DisciplineSummary(DisciplineSummaryViewModel model)
        {
            if (model != null)
            {
                var today = DateTime.Today;
                int month = DateTime.Today.Month;

                if (month >= 1 && month <= 6)
                {
                    model.FromDate = new DateTime(today.Year, 1, 1);
                    model.ToDate = new DateTime(today.Year, 6, DateTime.DaysInMonth(today.Year, 6));
                }
                else
                {
                    model.FromDate = new DateTime(today.Year, 6, 1);
                    model.ToDate = new DateTime(today.Year, 12, DateTime.DaysInMonth(today.Year, 12));
                }

                model.Teams = GetAllTeam();
                model.DisciplineTypes = GetDisciplineType();
            }

            return View(model);
        }

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
                ApproverId = CurrentUser.Id
            };

            IList<DisciplineSummaryModel> disciplines = disciplineService.GetDisciplineSummary(criteria);

            return PartialView("_ListDisciplineSummary", disciplines);
        }

        public ActionResult DisciplineDetail(DisciplineSummaryViewModel model)
        {
            if (model != null)
            {
                if (model.FromDate == default(DateTime))
                {
                    var today = DateTime.Today;
                    int month = DateTime.Today.Month;

                    if (month >= 1 && month <= 6)
                    {
                        model.FromDate = new DateTime(today.Year, 1, 1);
                        model.ToDate = new DateTime(today.Year, 6, DateTime.DaysInMonth(today.Year, 6));
                    }
                    else
                    {
                        model.FromDate = new DateTime(today.Year, 6, 1);
                        model.ToDate = new DateTime(today.Year, 12, DateTime.DaysInMonth(today.Year, 12));
                    }
                }

                model.Teams = GetAllTeam();
                model.DisciplineTypes = GetDisciplineType();
            }

            return View(model);
        }

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

            return PartialView("_ListDisciplineDetail", disciplines);
        }

        public ActionResult PartialViewOtherDiscipline(int employeeId, DateTime fromDate, DateTime toDate)
        {
            var criteria = new GetOtherDisciplineOfEmployeeCriteria()
            {
                EmployeeId = employeeId,
                FromDate = fromDate,
                ToDate = toDate
            };

            IList<OtherDisciplineModel> otherDisciplines = disciplineService.GetOtherDiscipline(criteria);

            return PartialView("_ViewOtherDiscipline", otherDisciplines);
        }

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

        [HttpPost]
        [Authorize(Roles = "SystemAdmin")]
        [HRAuthorize(FunctionKey = FunctionKey.Discipline, Policy = Policy.Create)]
        public JsonResult AddOtherDiscipline(AddOtherDisciplineViewModel model)
        {
            var criteria = new InsertOtherDisciplineCriteria
            {
                EmployeeId = model.EmployeeId,
                WorkDate = model.WorkDate,
                DisciplineTypeId = model.DisciplineTypeId,
                Comment = model.Comment,
                CreatedBy = CurrentUser.Id
            };

            CommandResult commandResult = commandService.Insert(criteria);

            return Json(commandResult);
        }

        [HttpPost]
        [Authorize(Roles = "SystemAdmin")]
        [HRAuthorize(FunctionKey = FunctionKey.Discipline, Policy = Policy.Delete)]
        public string DeleteOtherDiscipline(DeleteOtherDisciplineCriteria criteria)
        {
            if (criteria != null)
            {
                criteria.ModifiedBy = CurrentUser.Id;
                CommandResult commandResult = commandService.Delete(criteria);

                if (commandResult.Code > 0)
                {
                    return commandResult.Message;
                }
            }

            return string.Empty;
        }

        [HttpPost]
        [Authorize(Roles = "Approver")]
        [HRAuthorize(FunctionKey = FunctionKey.Discipline, Policy = Policy.Edit)]
        public JsonResult UpdateIgnoredDiscipline(IList<IgnoredDisciplineModel> ignoredDisciplineDetails)
        {
            int roleId = CurrentUser.RoleList.FirstOrDefault(x => x == (int)RoleSystem.Approver);

            var criteria = new UpdateIgnoredDisciplineCriteria
            {
                IgnoredDisciplines = ignoredDisciplineDetails,
                ModifiedBy = CurrentUser.Id,
                RoleId = roleId
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

        private IList<DisciplineTypeModel> GetDisciplineType()
        {
            IList<DisciplineTypeModel> disciplineTypes = disciplineService.GetDisciplineType(new GetDisciplineTypeCriteria());

            return disciplineTypes;
        }
    }
}