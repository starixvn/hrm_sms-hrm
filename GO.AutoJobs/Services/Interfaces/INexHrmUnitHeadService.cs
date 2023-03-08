namespace GO.AutoJobs.Services
{
    using System.Threading.Tasks;
    using HRM.UserControl.Core.Models;

    public interface INexHrmUnitHeadService
    {
        Task<CommandResult> SyncUniHeadInfoAsync();

        Task<CommandResult> SyncUniHeadScheduleAsync();
    }
}