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

                model.Teams = GetAllTeam();
                model.FromDate = today;
                model.ToDate = today;
            }

            return View(model);
        }

        public ActionResult ListDisciplineSummary(DisciplineSummaryViewModel model)
        {
            IList<DisciplineSummaryModel> disciplines = new List<DisciplineSummaryModel>();

            return PartialView("_ListDisciplineSummary", disciplines);
        }

        public IList<OtherViolationModel> GetOtherViolation(int employeeId, DateTime fromDate, DateTime toDate)
        {
            var criteria = new GetOtherViolationOfEmployeeCriteria()
            {
                EmployeeId = employeeId,
                FromDate = fromDate,
                ToDate = toDate
            };

            IList<OtherViolationModel> violations = _disciplineService.GetOtherViolation(criteria);

            return violations;
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
    }
}