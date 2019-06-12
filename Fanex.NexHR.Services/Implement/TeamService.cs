namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using Core.Services;
    using Fanex.NexHR.Core.Models;
    using Fanex.NexHR.Core.XMLExtensions;
    using Fanex.NexHR.Criteria;
    using Fanex.NexHR.Criteria.Supervisor;
    using Fanex.NexHR.Criteria.Team;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Section;
    using Fanex.NexHR.Models.Supervisor;
    using Fanex.NexHR.Repositories;

    public class TeamService : ITeamService
    {
        private readonly ITeamRepository teamRepository;
        private readonly IBaseService baseService;

        public TeamService(
            ITeamRepository teamRepository,
            IBaseService baseService)
        {
            this.teamRepository = teamRepository;
            this.baseService = baseService;
        }

        public Team GetById(GetGroupByIdCriteria criteria)
        {
            Team team = new Team();
            if (criteria != null && criteria.IsValid())
            {
                team = teamRepository.GetById(criteria);
            }

            return team;
        }

        public IList<Team> GetAll(GetAllGroupCriteria criteria)
        {
            IList<Team> teams = null;

            if (criteria != null && criteria.IsValid())
            {
                teams = teamRepository.GetAll(criteria);
            }

            return teams ?? new List<Team>();
        }

        public IList<Team> GetBySupervisor(GetGroupBySupervisorCriteria criteria)
        {
            return baseService.GetData<Team>(criteria).ToList();
        }

        public IList<SupervisorModel> GetTeamSupervisor(GetTeamSupervisorCriteria criteria)
        {
            return baseService.GetData<SupervisorModel>(criteria).ToList();
        }

        public IEnumerable<TeamSupervisorModel> GetSupervisorByTeamId(int id)
        {
            var criteria = new GetSupervisorByTeamIdCriteria
            {
                GroupId = id
            };
            return baseService.GetData<TeamSupervisorModel>(criteria).ToList();
        }

        public IEnumerable<Supervisor> GetDisciplineManagerBySectionId(int sectionId)
        {
            var criteria = new GetDisciplineManagerByTeamIdCriteria
            {
                GroupId = sectionId
            };
            return baseService.GetData<Supervisor>(criteria).ToList();
        }

        public CommandResult EditSectionSupervisor(int actorId, int sectionId, short roleId, IEnumerable<EditSectionSupervisor> supervisors)
        {
            var criteria = new EditSectionSupervisorCriteria
            {
                GroupId = sectionId,
                ActorId = actorId,
                RoleId = roleId,
                SupervisorXml = supervisors.ToXML().ToString()
            };
            return baseService.ExecuteNonQuery(criteria);
        }
    }
}