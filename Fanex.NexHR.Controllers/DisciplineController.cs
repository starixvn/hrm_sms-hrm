namespace Fanex.NexHR.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Fanex.NexHR.Criteria.Discipline;
    using Fanex.NexHR.Criteria.Team;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Discipline;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Implement;
    using Fanex.NexHR.Services.Interfaces;
    using Fanex.NexHR.ViewModels.Discipline;

    [Authorize]
    public class DisciplineController : BaseController
    {
        private readonly ITeamService _teamService;
        private readonly IDisciplineService _disciplineService;

        public DisciplineController()
            : this(null, null)
        {
        }

        public DisciplineController(
            ITeamService teamService,
            IDisciplineService disciplineService)
        {
            _teamService = teamService ?? new TeamService();
            _disciplineService = disciplineService ?? new DisciplineService();
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

        public ActionResult ListDisciplineSummary(DisciplineSummaryViewModel model)
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

            IList<DisciplineSummaryModel> disciplines = _disciplineService.GetDisciplineSummary(criteria);

            return PartialView("_ListDisciplineSummary", disciplines);
        }

        public ActionResult PartialViewOtherViolation(int employeeId, DateTime fromDate, DateTime toDate)
        {
            var criteria = new GetOtherViolationOfEmployeeCriteria()
            {
                EmployeeId = employeeId,
                FromDate = fromDate,
                ToDate = toDate
            };

            IList<OtherViolationModel> violations = _disciplineService.GetOtherViolation(criteria);

            return PartialView("_ViewOtherViolation", violations);
        }

        public ActionResult PartialAddOtherViolation(string employee, int? employeeId = null)
        {
            var criteria = new GetOtherViolationTypeCriteria();
            IList<DisciplineTypeModel> otherViolationTypes = _disciplineService.GetOtherViolationType(criteria);

            var model = new AddOtherViolationViewModel
            {
                EmployeeId = employeeId ?? -1,
                Employee = employee,
                WorkDate = DateTime.Now,
                OtherViolationTypes = otherViolationTypes
            };

            return PartialView("_AddOtherViolation", model);
        }

        [HttpPost]
        public JsonResult AddOtherViolation(AddOtherViolationViewModel model)
        {
            var criteria = new InsertOtherViolationCriteria
            {
                EmployeeId = model.EmployeeId,
                WorkDate = model.WorkDate,
                DisciplineTypeId = model.OtherViolationTypeId,
                Comment = model.Comment,
                CreatedBy = CurrentUser.Id
            };

            CommandResult commandResult = _disciplineService.InsertOtherViolation(criteria);

            return Json(commandResult);
        }

        private IList<Team> GetAllTeam()
        {
            IList<Team> teamList = _teamService.GetAll(new GetAllGroupCriteria());

            return teamList;
        }

        private IList<DisciplineTypeModel> GetDisciplineType()
        {
            IList<DisciplineTypeModel> disciplineTypes = _disciplineService.GetDisciplineType(new GetDisciplineTypeCriteria());

            return disciplineTypes;
        }
    }
}