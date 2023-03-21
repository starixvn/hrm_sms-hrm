namespace GO.AutoJobs.Services
{
    using System.Threading.Tasks;
    using HRM.UserControl.Core.Models;

    public interface INexHrmService
    {
        Task<CommandResult> SyncEmployeeInfoAsync(bool isNexcel = true);

        Task<CommandResult> SyncTeamAsync(bool isNexcel = true);

        Task<CommandResult> SyncUniHeadInfoAsync();

        Task<CommandResult> SyncUniHeadScheduleAsync();
    }
}