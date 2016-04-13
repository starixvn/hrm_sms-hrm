namespace Fanex.NexHR.Controllers
{
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Fanex.NexHR.Criteria.Team;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Discipline;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interfaces;
    using Fanex.NexHR.ViewModels.Discipline;

    public class DisciplineController : BaseController
    {
        private readonly ITeamService _teamService;

        public DisciplineController()
            : this(null)
        {
        }

        public DisciplineController(ITeamService teamService)
        {
            _teamService = teamService ?? new TeamService();
        }

        public ActionResult DisciplineSummary(DisciplineSummaryViewModel model)
        {
            if (model != null)
            {
                model.Teams = GetAllTeam();
            }

            return View(model);
        }

        public ActionResult ListDisciplineSummary(DisciplineSummaryViewModel model)
        {
            IList<DisciplineSummaryModel> disciplines = new List<DisciplineSummaryModel>();

            return PartialView("_ListDisciplineSummary", disciplines);
        }

        private IList<Team> GetAllTeam()
        {
            IList<Team> teamList = _teamService.GetAll(new GetAllGroupCriteria());

            return teamList;
        }
    }
}