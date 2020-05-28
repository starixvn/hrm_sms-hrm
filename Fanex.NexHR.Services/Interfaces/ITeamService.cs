namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Core.Models;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Group;
    using Fanex.NexHR.Models.Supervisor;

    public interface ITeamService
    {
        Team GetById(int id);

        IList<Team> GetAll(bool isDeleted = false);

        CommandResult Delete(int actorId, int id);

        IList<Team> GetBySupervisor(int supervisorId);

        IList<SupervisorModel> GetTeamSupervisorByRoleId(int sectionId, RoleSystem roleId);

        IEnumerable<TeamSupervisorModel> GetSupervisorByTeamId(int id);

        IEnumerable<Supervisor> GetDisciplineManagerByTeamId(int sectionId);

        CommandResult EditTeamSupervisor(int actorId, int sectionId, short roleId, IEnumerable<EditTeamSupervisor> supervisors);

        IEnumerable<DisciplineRule> GetDisciplineRuleByTeam(int sectionId);

        CommandResult EditDisciplineRule(int actorId, int sectionId, IEnumerable<DisciplineRule> disciplineRules);
    }
}