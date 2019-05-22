namespace Fanex.NexHR.Services
{
    public interface IBackgroundJobService
    {
        void InsertScheduleByDefaultShift();

        void InsertScheduleByWeeklyDefaultShift();
    }
}