namespace Fanex.NexHR.Services
{
    public interface IBackgroundJobService
    {
        void PublishSchedule();

        void InsertScheduleByDefaultShift();

        void InsertScheduleByWeeklyDefaultShift();

        void InsertHolidayAutomatically();
    }
}