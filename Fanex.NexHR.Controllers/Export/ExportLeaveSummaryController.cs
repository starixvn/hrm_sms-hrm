namespace Fanex.NexHR.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Criteria;
    using Criteria.AnnualLeave;
    using Criteria.Team;
    using Models;
    using NPOI.HSSF.UserModel;
    using Services;
    using Services.ExportExcel;
    using ViewModels;

    public class ExportLeaveSummaryController : BaseExportController
    {
        private readonly IReportService reportService;
        private readonly IExportExcelService exportExcelService;
        private readonly ITeamService teamService;
        private readonly IExportExcelUnpaidLeaveSummaryReportService exportExcelUnpaidLeaveSummaryReportService;

        public ExportLeaveSummaryController(
            IReportService reportService,
            IExportExcelService exportExcelService,
            ITeamService teamService,
            IExportExcelUnpaidLeaveSummaryReportService exportExcelUnpaidLeaveSummaryReportService)
        {
            this.reportService = reportService;
            this.exportExcelService = exportExcelService;
            this.teamService = teamService;
            this.exportExcelUnpaidLeaveSummaryReportService = exportExcelUnpaidLeaveSummaryReportService;
        }

        public ActionResult AnnualLeaveSummary(LeaveSummaryViewModel model)
        {
            string fileName = "LeaveSummary_" + DateTime.Now.ToString("yyyyMMdd") + ".xls";
            byte? isLocal = null;
            if (model.Category != null)
            {
                isLocal = (byte)model.Category;
            }

            var criteria = new GetAnnualLeaveBalanceCriteria
            {
                Year = model.Year,
                EmpId = model.EmployeeId != -1 ? new int?(model.EmployeeId) : null,
                GroupIds = model.SectionIds,
                IsLocal = isLocal,
                Role = string.Empty,
                SupervisorId = null
            };
            IList<LeaveSummary> leaveSummary = reportService.GetAnnualLeaveBalanceReport(criteria);
            var getAllGroupCriteria = new GetAllGroupCriteria();
            IList<Team> teams = teamService.GetAll(getAllGroupCriteria);
            IList<LeaveSummaryByTeam> leaveSummaryByTeamList = reportService.GetLeaveByTeam(leaveSummary, teams);

            HSSFWorkbook hssfworkbook = exportExcelService.ExportLeaveSummary(leaveSummaryByTeamList, model.Year);

            return ExcelFile(hssfworkbook, fileName);
        }

        public ActionResult UnpaidLeaveSummary(LeaveSummaryViewModel model)
        {
            var criteria = new GetUnpaidLeaveCriteria
            {
                Year = model.Year,
                EmpId = model.EmployeeId != -1 ? new int?(model.EmployeeId) : null,
                GroupIds = model.SectionIds,
                IsLocal = (byte?)model.Category,
                SupervisorId = CurrentUser.Id,
                LeaveType = (int)model.LeaveType
            };
            IList<LeaveSummary> leaveSummary = reportService.GetUnpaidLeave(criteria);
            IList<Team> teams = teamService.GetAll(new GetAllGroupCriteria());
            IList<LeaveSummaryByTeam> leaveSummaryByTeamList = reportService.GetLeaveByTeam(leaveSummary, teams);
            exportExcelUnpaidLeaveSummaryReportService.Export(leaveSummaryByTeamList, model.Year, model.LeaveType);

            return new EmptyResult();
        }
    }
}