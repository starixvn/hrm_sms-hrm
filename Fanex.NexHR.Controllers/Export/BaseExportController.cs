namespace Fanex.NexHR.Controllers
{
    using NPOI.HSSF.UserModel;
    using System.IO;
    using System.Web.Mvc;

    public class BaseExportController : BaseController
    {
        protected ActionResult ExcelFile(HSSFWorkbook hssfworkbook, string fileName)
        {
            using (var output = new MemoryStream())
            {
                hssfworkbook.Write(output);

                return File(output.ToArray(), "application/vnd.ms-excel", fileName);
            }
        }
    }
}