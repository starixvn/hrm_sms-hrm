namespace GO.AutoJobs.Services
{
    using System.Threading.Tasks;
    using HRM.UserControl.Core.Models;

    public interface IUserSiteService
    {
        Task<CommandResult> CopyUserToAnotherSiteAsync();
    }
}