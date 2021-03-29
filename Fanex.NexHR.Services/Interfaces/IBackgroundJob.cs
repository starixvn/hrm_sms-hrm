namespace Fanex.NexHR.Services
{
    public interface IBackgroundJobService
    {
        void ImportTimeRecord();

        void PublishSchedule();

        void InsertScheduleByDefaultShift();

        void InsertScheduleByWeeklyDefaultShift();

        void InsertHolidayAutomatically();

        void UpdateWorkingHourPerDay();

        void UpdateAnnualLeaveBalance();

        void UpdateAnnualLeaveBalanceForNewEmployee();
    }
}