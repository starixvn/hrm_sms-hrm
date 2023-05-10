namespace GO.AutoJobs.Services
{
    using System.Threading.Tasks;
    using GO.Core.Models;

    public interface INexHrmService
    {
        Task<CommandResult> SyncEmployeeInfoAsync(bool isNexcel = true);

        Task<CommandResult> SyncTeamAsync(bool isNexcel = true);

        Task<CommandResult> SyncSectionAsync(bool isNexcel = true);

        Task<CommandResult> SyncJobTitleAsync(bool isNexcel = true);

        Task<CommandResult> SyncUniHeadInfoAsync();

        Task<CommandResult> SyncUniHeadScheduleAsync();
    }
}