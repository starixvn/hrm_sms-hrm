namespace Fanex.NexHR.Services.Interfaces
{
    using System.Collections.Generic;
    using Fanex.NexHR.Criteria.Supervisor;
    using Fanex.NexHR.Criteria.Team;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Supervisor;

    public interface ITeamService
    {
        Team GetById(GetGroupByIdCriteria criteria);

        IList<Team> GetAll(GetAllGroupCriteria criteria);

        IList<Team> GetBySupervisor(GetGroupBySupervisorCriteria criteria);

        IList<SupervisorModel> GetTeamSupervisor(GetTeamSupervisorCriteria criteria);

        IList<TeamSupervisorModel> GetSupervisorByTeamId(GetSupervisorByTeamIdCriteria criteria);
    }
}