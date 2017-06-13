namespace Fanex.NexHR.Services.ExportExcel
{
    using Core;
    using Criteria.Discipline;
    using Models.Discipline;
    using Resources;
    using System.Collections.Generic;
    using System.Web;

    public class ExportDisciplineDetailService : IExportDisciplineDetailService
    {
        private const string TemplatePath = @"~\ExcelTemplates\DisciplineDetailReport.xls";
        private const int HeaderRow = 2;
        private const int BodyRow = 3;
        private const int WeekendRow = 4;
        private const int TotalRow = 24;

        public void Export(GetDisciplineDetailCriteria criteria, IList<DisciplineDetailModel> disciplines)
        {
            string templatePath = HttpContext.Current.Server.MapPath(TemplatePath),
                reportName = Labels.DisciplineDetailReportName,
                reportTitle,
                startDate = criteria.FromDate.ToString(Constants.DateFormat),
                endDate = criteria.ToDate.ToString(Constants.DateFormat);

            if (criteria.EmployeeId != null)
            {
                reportTitle = string.Format(Labels.DisciplineDetailReportTitle, criteria.EmployeeName, startDate, endDate);
            }
            else if (criteria.GroupId != null)
            {
                reportTitle = string.Format(Labels.DisciplineDetailReportTitle, criteria.GroupName, startDate, endDate);
            }
            else
            {
                reportTitle = string.Format(Labels.DisciplineDetailReportTitleNoPrefix, startDate, endDate);
            }

            var writer = new ExcelTemplateWriter(templatePath, reportName, Labels.DisciplineDetailReportSheet);

            // Fill report title.
            writer.WriteRow(0, new object[] { reportTitle });
            writer.CopyRows(1, 1);

            // Fill report header.
            writer.CopyRows(HeaderRow, 1);

            FillContentRow(disciplines, ref writer);
            writer.SyncColumnsWidth(TotalRow);
            writer.CreateFreezePane(6, 3);

            writer.Render();
        }

        private void FillContentRow(IList<DisciplineDetailModel> disciplines, ref ExcelTemplateWriter writer)
        {
            int rowIndex = 1;

            foreach (DisciplineDetailModel discipline in disciplines)
            {
                FillRow(discipline, ref writer, ref rowIndex);
            }
        }

        private void FillRow(DisciplineDetailModel discipline, ref ExcelTemplateWriter writer, ref int rowIndex)
        {
            object record = CreateRecord(discipline, ref rowIndex);

            if (discipline.IsWeekend)
            {
                writer.WriteRow(WeekendRow, record);
            }
            else
            {
                writer.WriteRow(BodyRow, record);
            }
        }

        private object CreateRecord(DisciplineDetailModel discipline, ref int rowIndex)
        {
            decimal systemSubtractScore = discipline.SystemSubtractScore > 0 ? -discipline.SystemSubtractScore : 0,
                approverSubtractScore = discipline.ApproverSubtractScore > 0 ? -discipline.ApproverSubtractScore : 0;

            var record = new
            {
                Shift = discipline.Employee.Code,
                Nick = discipline.Employee.NickName,
                FullName = discipline.Employee.FullName,
                Category = discipline.Employee.IsLocal ? Labels.Local : Labels.Expat,
                Section = discipline.Employee.GroupName,
                Date = discipline.WorkDate,
                ShiftName = discipline.ShiftName,
                CheckIn = discipline.CheckIn.HasValue ? discipline.CheckIn.Value.ToString(Constants.TimeFormat) : string.Empty,
                CheckOut = discipline.CheckOut.HasValue ? discipline.CheckOut.Value.ToString(Constants.TimeFormat) : string.Empty,
                TotalHours = !string.IsNullOrEmpty(discipline.TotalHours) ? discipline.TotalHours : string.Empty,
                MissingCheckInOut = discipline.MissingCheckInOut.Value > 0 ? discipline.MissingCheckInOut.Value.ToString() : string.Empty,
                WorkingUnderTime = discipline.WorkingUnderTime.Value > 0 ? "1" : string.Empty,
                LateIn = !string.IsNullOrEmpty(discipline.MinutesLateIn) ? discipline.MinutesLateIn : string.Empty,
                EarlyOut = !string.IsNullOrEmpty(discipline.MinutesEarlyOut) ? discipline.MinutesEarlyOut : string.Empty,
                Absent = discipline.Absent.Value > 0 ? discipline.Absent.Value.ToString() : string.Empty,
                UnpaidLeave = discipline.UnpaidLeave.Value > 0 ? discipline.UnpaidLeave.Value.ToString() : string.Empty,
                UnpaidLeaveAbsent = discipline.UnpaidLeaveAbsent.Value > 0 ? discipline.UnpaidLeaveAbsent.Value.ToString() : string.Empty,
                ChangeShift = discipline.ChangeShift.Value > 0 ? discipline.ChangeShift.Value.ToString() : string.Empty,
                UnplannedLeave = discipline.UnplannedLeave.Value > 0 ? discipline.UnplannedLeave.Value.ToString() : string.Empty,
                WeekendLeave = discipline.WeekendLeave.Value > 0 ? discipline.WeekendLeave.Value.ToString() : string.Empty,
                SystemSubtractScore = systemSubtractScore,
                ApproverSubtractScore = approverSubtractScore,
                ApproverComment = discipline.ApproverComment,
                EmployeeComment = discipline.EmployeeComment
            };
            rowIndex++;

            return record;
        }
    }
}