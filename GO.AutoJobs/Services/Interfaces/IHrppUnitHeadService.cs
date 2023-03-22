namespace GO.AutoJobs.Services
{
    using System.Threading.Tasks;
    using GO.Core.Models;

    public interface IHrppUnitHeadService
    {
        Task<CommandResult> SyncUniHeadInfoAsync();

        Task<CommandResult> SyncUniHeadScheduleAsync();
    }
}