namespace Fanex.NexHR.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Fanex.NexHR.Core.Services;
    using Fanex.NexHR.Criteria.EmployeeWorkFromHome;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Services;
    using NPOI.HSSF.UserModel;

    public class ExportEmployeeWorkFromHomeController : BaseExportController
    {
        private readonly IBaseService baseService;
        private readonly IExportExcelService exportExcelService;

        public ExportEmployeeWorkFromHomeController(
            ISessionService sessionService,
            IExportExcelService exportExcelService,
            IBaseService baseService) : base(sessionService)
        {
            this.baseService = baseService;
            this.exportExcelService = exportExcelService;
        }

        public ActionResult EmployeeWorkFromHome(GetEmployeeWorkFromHomeCriteria criteria)
        {
            var employees = baseService.GetData<EmployeeWorkFromHome>(criteria);
            string fileName = "EmployeeWorkFromHome_" + DateTime.Now.ToString("yyyyMMdd") + ".xls";

            HSSFWorkbook hssfworkbook = exportExcelService.ExportEmployeeWfh(employees);

            return ExcelFile(hssfworkbook, fileName);
        }
    }
}