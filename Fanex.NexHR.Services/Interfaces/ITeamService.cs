namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Core.Models;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Section;
    using Fanex.NexHR.Models.Supervisor;

    public interface ITeamService
    {
        Team GetById(int id);

        IList<Team> GetAll(bool isDeleted = false);

        CommandResult Delete(int actorId, int id);

        IList<Team> GetBySupervisor(int supervisorId);

        IList<SupervisorModel> GetSectionSupervisorByRoleId(int sectionId, RoleSystem roleId);

        IEnumerable<TeamSupervisorModel> GetSupervisorByTeamId(int id);

        IEnumerable<Supervisor> GetDisciplineManagerBySectionId(int sectionId);

        CommandResult EditSectionSupervisor(int actorId, int sectionId, short roleId, IEnumerable<EditSectionSupervisor> supervisors);
    }
}