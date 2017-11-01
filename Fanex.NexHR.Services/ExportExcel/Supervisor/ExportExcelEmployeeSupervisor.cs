namespace Fanex.NexHR.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using ExportExcel;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Models.Supervisor;

    public class ExportExcelEmployeeSupervisor : IExportExcelEmployeeSupervisor
    {
        private const int RowStartHeader = 2;
        private const int RowStartBody = 3;
        private const int TotalColumns = 10;
        private const string EmployeeSupervisorTemplatePath = @"~\ExcelTemplates\EmployeeSupervisorTemplate.xls";

        public void Export(IList<SupervisorReportModel> supervisors)
        {
            string templatePath = HttpContext.Current.Server.MapPath(EmployeeSupervisorTemplatePath);
            string fileName = "EmployeeSupervisor_" + DateTime.Now.ToString("yyyyMMdd");

            var writer = new ExcelTemplateWriter(templatePath, fileName, "Employee Supervisor");

            /// Fill report title.
            writer.CopyRows(0, 2);
            /// Fill report header.
            writer.CopyRows(RowStartHeader, 1);

            FillContentRowReport(supervisors, ref writer);
            writer.SyncColumnsWidth(TotalColumns);
            writer.CreateFreezePane(0, RowStartBody);

            writer.Render();
        }

        private void FillContentRowReport(IList<SupervisorReportModel> supervisors, ref ExcelTemplateWriter writer)
        {
            int rowIndex = 1;

            foreach (SupervisorReportModel supervisor in supervisors)
            {
                FillRowReport(supervisor, ref writer, ref rowIndex);
            }
        }

        private void FillRowReport(SupervisorReportModel supervisor, ref ExcelTemplateWriter writer, ref int rowIndex)
        {
            object record = CreateRecordReport(supervisor, rowIndex);
            ExportExcelService.WriteRowExcelReport(writer, ref rowIndex, RowStartBody, record);

            rowIndex++;
        }

        private object CreateRecordReport(SupervisorReportModel supervisor, int rowIndex)
        {
            var record = new
            {
                Count = rowIndex,
                NickName = supervisor.NickName,
                FullName = supervisor.FullName,
                Section = supervisor.Section.Code,
                Roles = supervisor.Roles2String,
                Verifier = string.Join(", ", supervisor.Supervisors.Where(x => x.ActionType == (int)SupervisorActionType.Verifier).Select(x => x.NickName)),
                Approver = string.Join(", ", supervisor.Supervisors.Where(x => x.ActionType == (int)SupervisorActionType.Approver).Select(x => x.NickName)),
                ShiftArrangement = string.Join(", ", supervisor.Supervisors.Where(x => x.ActionType == (int)SupervisorActionType.ShiftArrangement).Select(x => x.NickName)),
                OnBehalfRequest = string.Join(", ", supervisor.Supervisors.Where(x => x.ActionType == (int)SupervisorActionType.OnBehalfRequest).Select(x => x.NickName)),
                DisciplineManager = string.Join(", ", supervisor.Supervisors.Where(x => x.ActionType == (int)SupervisorActionType.DisciplineManager).Select(x => x.NickName))
            };

            return record;
        }
    }
}