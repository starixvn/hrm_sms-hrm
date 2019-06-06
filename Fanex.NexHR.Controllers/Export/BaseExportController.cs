namespace Fanex.NexHR.Controllers
{
    using System.IO;
    using System.Web.Mvc;
    using Fanex.NexHR.Core.Services;
    using NPOI.HSSF.UserModel;

    public class BaseExportController : BaseController
    {
        public BaseExportController(ISessionService sessionService) : base(sessionService)
        {
        }

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