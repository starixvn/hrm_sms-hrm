namespace GO.AutoJobs.Services
{
    using System.Threading.Tasks;

    public interface IManpowerForecastService
    {
        Task SendNotificationAsync();
    }
}