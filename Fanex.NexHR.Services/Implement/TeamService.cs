namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Core.Models;
    using Fanex.NexHR.Core.Services;
    using Fanex.NexHR.Core.XMLExtensions;
    using Fanex.NexHR.Criteria;
    using Fanex.NexHR.Criteria.Section;
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

        public Team GetById(int id)
        {
            var criteria = new GetGroupByIdCriteria
            {
                GroupId = id
            };
            return teamRepository.GetById(criteria);
        }

        public IList<Team> GetAll(bool isDeleted = false)
        {
            var criteria = new GetAllGroupCriteria
            {
                IsDeleted = isDeleted
            };
            IList<Team> teams = teamRepository.GetAll(criteria);

            return teams ?? new List<Team>();
        }

        public CommandResult Delete(int actorId, int id)
        {
            var criteria = new DeleteGroupCriteria
            {
                GroupId = id,
                ModifiedBy = actorId
            };
            return baseService.ExecuteNonQuery(criteria);
        }

        public IList<Team> GetBySupervisor(int supervisorId)
        {
            var criteria = new GetGroupBySupervisorCriteria
            {
                SupervisorId = supervisorId
            };
            return baseService.GetData<Team>(criteria).ToList();
        }

        public IList<SupervisorModel> GetSectionSupervisorByRoleId(int sectionId, RoleSystem roleId)
        {
            var criteria = new GetSectionSupervisorByRoleIdCriteria
            {
                GroupId = sectionId,
                Role = (short)roleId
            };
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