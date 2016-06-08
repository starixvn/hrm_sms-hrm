namespace Fanex.NexHR.Services
{
    using Fanex.NexHR.Criteria.Supervisor;
    using Fanex.NexHR.Criteria.Team;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Supervisor;
    using Fanex.NexHR.Repositories;
    using Fanex.NexHR.Repositories.Interfaces;
    using Fanex.NexHR.Services.Interfaces;
    using System.Collections.Generic;
    using System.Linq;

    public class TeamService : BaseService, ITeamService
    {
        private readonly ITeamRepository teamRepository;

        public TeamService()
            : this(null)
        {
        }

        public TeamService(ITeamRepository teamRepository)
        {
            this.teamRepository = teamRepository ?? new TeamRepository();
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
            IList<Team> teams = GetData<Team>(criteria).ToList();
            return teams;
        }

        public IList<SupervisorModel> GetTeamSupervisor(GetTeamSupervisorCriteria criteria)
        {
            IList<SupervisorModel> supervisors = GetData<SupervisorModel>(criteria).ToList();
            return supervisors;
        }

        public IList<TeamSupervisorModel> GetSupervisorByTeamId(GetSupervisorByTeamIdCriteria criteria)
        {
            IList<TeamSupervisorModel> supervisors = GetData<TeamSupervisorModel>(criteria).ToList();
            return supervisors;
        }
    }
}