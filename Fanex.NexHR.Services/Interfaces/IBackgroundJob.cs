namespace Fanex.NexHR.Services
{
    using System.Threading.Tasks;

    public interface IBackgroundJobService
    {
        void ImportTimeRecord();

        void PublishSchedule();

        void InsertScheduleByDefaultShift();

        void InsertScheduleByWeeklyDefaultShift();

        void InsertHolidayAutomatically();

        void UpdateWorkingHourPerDay();

        void SendMailRemindUserToUseAnnualLeave();

        void UpdateAnnualLeaveBalance();

        void UpdateAnnualLeaveBalanceForNewEmployee();

        Task SyncWorkingShiftAsync();
    }
}