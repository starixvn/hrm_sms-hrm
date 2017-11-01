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

    public class ExportExcelSectionSupervisorService : IExportExcelSectionSupervisorService
    {
        private const int RowStartHeader = 2;
        private const int RowStartBody = 3;
        private const int TotalColumns = 10;
        private const string SectionSupervisorTemplatePath = @"~\ExcelTemplates\SectionSupervisorTemplate.xls";

        public void Export(IList<SectionSupervisorReportModel> supervisors)
        {
            string templatePath = HttpContext.Current.Server.MapPath(SectionSupervisorTemplatePath);
            string fileName = "SectionSupervisor_" + DateTime.Now.ToString("yyyyMMdd");

            var writer = new ExcelTemplateWriter(templatePath, fileName, "Section Supervisor");

            /// Fill report title.
            writer.CopyRows(0, 2);
            /// Fill report header.
            writer.CopyRows(RowStartHeader, 1);

            FillContentRowReport(supervisors, ref writer);
            writer.SyncColumnsWidth(TotalColumns);
            writer.CreateFreezePane(0, RowStartBody);

            writer.Render();
        }

        private void FillContentRowReport(IList<SectionSupervisorReportModel> supervisors, ref ExcelTemplateWriter writer)
        {
            int rowIndex = 1;

            foreach (SectionSupervisorReportModel supervisor in supervisors)
            {
                FillRowReport(supervisor, ref writer, ref rowIndex);
            }
        }

        private void FillRowReport(SectionSupervisorReportModel supervisor, ref ExcelTemplateWriter writer, ref int rowIndex)
        {
            object record = CreateRecordReport(supervisor, rowIndex);
            ExportExcelService.WriteRowExcelReport(writer, ref rowIndex, RowStartBody, record);
        }

        private object CreateRecordReport(SectionSupervisorReportModel supervisor, int rowIndex)
        {
            var record = new
            {
                Count = rowIndex,
                Section = supervisor.Code,
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