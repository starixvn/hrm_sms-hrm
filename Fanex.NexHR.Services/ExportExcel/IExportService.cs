namespace Fanex.NexHR.Services.ExportExcel
{
    using NPOI.HSSF.UserModel;

    public interface IExportService
    {
        HSSFWorkbook Export();
    }
}
