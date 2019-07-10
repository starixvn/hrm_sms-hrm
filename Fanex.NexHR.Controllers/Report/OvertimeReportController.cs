namespace Fanex.NexHR.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Web.Mvc;
    using Fanex.NexHR.Core.Constants;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Core.Filters;
    using Fanex.NexHR.Core.Services;
    using Fanex.NexHR.Criteria;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Report.Overtime;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.ExportExcel;
    using Fanex.NexHR.ViewModels.Report.Overtime;
    using NPOI.HSSF.UserModel;

    [HRMPermission(Policy.Read, FunctionKey.ReportHome)]
    public class OvertimeReportController : BaseController
    {
        private readonly ITeamService teamService;
        private readonly IOvertimeRequestService overtimeService;
        private readonly IOvertimeReportService overtimeReportService;
        private readonly IExportExcelService exportExcelService;

        public OvertimeReportController(
            ISessionService sessionService,
            ITeamService teamService,
            IOvertimeRequestService overtimeService,
            IOvertimeReportService overtimeReportService,
            IExportExcelService exportExcelService) : base(sessionService)
        {
            this.teamService = teamService;
            this.overtimeService = overtimeService;
            this.overtimeReportService = overtimeReportService;
            this.exportExcelService = exportExcelService;
        }

        public ActionResult MonthlyOvertime()
        {
            var today = DateTime.Today;
            var model = new MonthlyFilterViewModel
            {
                Month = today.Month,
                Year = today.Year,
                Calendar = (byte)CalendarType.Fiscal
            };

            return View("~/Views/Report/Overtime/MonthlyOvertime.cshtml", model);
        }

        public ActionResult YearlyOvertime(YearlyOvertimeViewModel model)
        {
            IEnumerable<Team> sections = teamService.GetAll();
            model.Sections = sections.Select(x => new EntityBase<int> { Id = x.Id, Name = x.Name, Code = x.Code });
            var param = new GetYearlyOvertimeParam
            {
                Year = model.Year,
                Calendar = model.Calendar,
                SectionIds = model.SectionIds,
                EmployeeId = model.EmployeeId
            };
            model.Overtimes = overtimeReportService.GetYearlyOvertime(param);

            return View("~/Views/Report/Overtime/YearlyOvertime.cshtml", model);
        }

        public ActionResult ExportMonthlyOvertime(MonthlyFilterViewModel model)
        {
            DateTime requestDate = new DateTime(model.Year, model.Month, 1);

            string fileName = "Monthly Overtime Report " + requestDate.ToString("MMM-yyyy") + ".xls";
            var criteria = new GetOvertimeRecordCriteria
            {
                Month = requestDate,
                EmployeeId = model.EmployeeId,
                Calendar = model.Calendar
            };
            IList<OvertimeExcelReport> overtimes = overtimeService.OvertimeRecord(criteria);
            HSSFWorkbook hssfworkbook = exportExcelService.ExportEmployeeOvertimeRecord(overtimes, model.Calendar, model.FromDate, model.ToDate);

            using (var output = new MemoryStream())
            {
                hssfworkbook.Write(output);

                return File(output.ToArray(), "application/vnd.ms-excel", fileName);
            }
        }

        public ActionResult ExportYearlyOvertime(YearlyOvertimeViewModel model)
        {
            var param = new GetYearlyOvertimeParam
            {
                Year = model.Year,
                Calendar = model.Calendar,
                SectionIds = model.SectionIds,
                EmployeeId = model.EmployeeId
            };
            IEnumerable<YearlyOvertime> overtimes = overtimeReportService.GetYearlyOvertime(param);
            byte[] bytes = overtimeReportService.ExportYearlyOvertime(overtimes, param);

            var fileName = $"Yearly overtime report - {param.Year}.xlsx";
            return File(bytes, "application/xlsx", fileName);
        }
    }
}