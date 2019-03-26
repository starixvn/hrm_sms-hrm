namespace Fanex.NexHR.Services
{
    using NPOI.HSSF.UserModel;

    public interface IExportService
    {
        HSSFWorkbook Export();
    }
}