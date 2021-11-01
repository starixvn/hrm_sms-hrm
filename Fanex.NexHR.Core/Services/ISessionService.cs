namespace Fanex.NexHR.Core.Services
{
    using Fanex.NexHR.Core.Models;

    public interface ISessionService
    {
        EmployeeSession Employee { get; set; }
        bool IsShowLeftNavigation { get; set; }

        void ClearAll();
    }
}