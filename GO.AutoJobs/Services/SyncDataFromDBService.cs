namespace GO.AutoJobs.Services
{
    using System.Threading.Tasks;
    using GO.AutoJobs.Criteria.HRM;
    using GO.Core.Models;
    using GO.Core.Services;

    public class SyncDataFromDBService : ISyncDataFromDBService
    {
        private readonly IBaseService baseService;

        public SyncDataFromDBService(IBaseService baseService)
        {
            this.baseService = baseService;
        }

        public async Task<CommandResult> SyncEmployeeInfoAsync(short siteId, string dbName)
        {
            var command = new ImportEmployeeFromDBCommand
            {
                SiteId = siteId,
                DbName = dbName
            };
            return await baseService.ExecuteNonQueryAsync(command);
        }

        public async Task<CommandResult> SyncTeamAsync(short siteId, string dbName)
        {
            var command = new ImportTeamFromDBCommand
            {
                SiteId = siteId,
                DbName = dbName
            };
            return await baseService.ExecuteNonQueryAsync(command);
        }

        public async Task<CommandResult> SyncSectionAsync(short siteId, string dbName)
        {
            var command = new ImportSectionFromDBCommand
            {
                SiteId = siteId,
                DbName = dbName
            };
            return await baseService.ExecuteNonQueryAsync(command);
        }

        public async Task<CommandResult> SyncDepartmentAsync(short siteId, string dbName)
        {
            var command = new ImportDepartmentFromDBCommand
            {
                SiteId = siteId,
                DbName = dbName
            };
            return await baseService.ExecuteNonQueryAsync(command);
        }

        public async Task<CommandResult> SyncUnitAsync(short siteId, string dbName)
        {
            var command = new ImportUnitFromDBCommand
            {
                SiteId = siteId,
                DbName = dbName
            };
            return await baseService.ExecuteNonQueryAsync(command);
        }

        public async Task<CommandResult> SyncJobTitleAsync(short siteId, string dbName)
        {
            var command = new ImportJobTitleFromDBCommand
            {
                SiteId = siteId,
                DbName = dbName
            };
            return await baseService.ExecuteNonQueryAsync(command);
        }

        public async Task<CommandResult> SyncJobLevelAsync(short siteId, string dbName)
        {
            var command = new ImportJobLevelFromDBCommand
            {
                SiteId = siteId,
                DbName = dbName
            };
            return await baseService.ExecuteNonQueryAsync(command);
        }
    }
}