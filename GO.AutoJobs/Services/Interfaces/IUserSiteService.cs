namespace GO.AutoJobs.Services
{
    using System.Threading.Tasks;
    using GO.Core.Models;

    public interface IUserSiteService
    {
        Task<CommandResult> CopyUserToAnotherSiteAsync();
    }
}