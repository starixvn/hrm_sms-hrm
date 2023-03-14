namespace GO.AutoJobs.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using GO.AutoJobs.Criteria.UserSite;
    using GO.AutoJobs.Models.UserSite;
    using HRM.UserControl.Core.Models;
    using HRM.UserControl.Core.Services;

    public class UserSiteService : IUserSiteService
    {
        private readonly List<CopyUserToAnotherSiteSetting> settings;
        private readonly IBaseService baseService;

        public UserSiteService(
            List<CopyUserToAnotherSiteSetting> settings,
            IBaseService baseService)
        {
            this.settings = settings;
            this.baseService = baseService;
        }

        public async Task<CommandResult> CopyUserToAnotherSiteAsync()
        {
            var commandResult = new CommandResult { Code = 0 };
            foreach (var setting in settings)
            {
                var command = new CopyUserSiteToAnotherCommand
                {
                    FromSiteId = setting.FromSiteId,
                    ToSiteId = setting.ToSiteId
                };

                commandResult = await baseService.ExecuteNonQueryAsync(command);
            }

            return commandResult;
        }
    }
}