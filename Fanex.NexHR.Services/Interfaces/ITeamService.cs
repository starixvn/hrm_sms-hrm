namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using Fanex.NexHR.Core.Models;
    using Fanex.NexHR.Criteria.Supervisor;
    using Fanex.NexHR.Criteria.Team;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Section;
    using Fanex.NexHR.Models.Supervisor;

    public interface ITeamService
    {
        Team GetById(GetGroupByIdCriteria criteria);

        IList<Team> GetAll(GetAllGroupCriteria criteria);

        IList<Team> GetBySupervisor(GetGroupBySupervisorCriteria criteria);

        IList<SupervisorModel> GetTeamSupervisor(GetTeamSupervisorCriteria criteria);

        IEnumerable<TeamSupervisorModel> GetSupervisorByTeamId(int id);

        IEnumerable<DisciplineManager> GetDisciplineManagerBySectionId(int sectionId);

        CommandResult EditSectionSupervisor(int actorId, int sectionId, short roleId, IEnumerable<EditSectionSupervisor> supervisors);
    }
}