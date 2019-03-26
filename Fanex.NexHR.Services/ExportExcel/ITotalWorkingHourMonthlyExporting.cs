namespace Fanex.NexHR.Services
{
    using NPOI.HSSF.UserModel;

    public interface ITotalWorkingHourMonthlyExporting
    {
        HSSFWorkbook ExportTotalWorkingHourMonthly();
    }
}