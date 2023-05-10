namespace GO.AutoJobs.Services
{
    using System.Threading.Tasks;
    using GO.Core.Models;

    public interface ISyncDataFromDBService
    {
        Task<CommandResult> SyncEmployeeInfoAsync(short siteId, string dbName);

        Task<CommandResult> SyncTeamAsync(short siteId, string dbName);

        Task<CommandResult> SyncSectionAsync(short siteId, string dbName);

        Task<CommandResult> SyncDepartmentAsync(short siteId, string dbName);

        Task<CommandResult> SyncUnitAsync(short siteId, string dbName);

        Task<CommandResult> SyncJobTitleAsync(short siteId, string dbName);

        Task<CommandResult> SyncJobLevelAsync(short siteId, string dbName);
    }
}