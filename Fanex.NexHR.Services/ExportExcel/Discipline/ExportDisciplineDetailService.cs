namespace Fanex.NexHR.Services.ExportExcel
{
    using System.Collections.Generic;
    using System.Web;
    using Core;
    using Criteria.Discipline;
    using Models.Discipline;
    using Resources;

    public class ExportDisciplineDetailService : IExportDisciplineDetailService
    {
        private const string TemplatePath = @"~\ExcelTemplates\DisciplineDetailReport.xls";
        private const int HeaderRow = 2;
        private const int BodyRow = 3;
        private const int WeekendRow = 4;
        private const int TotalRow = 23;

        public void Export(GetDisciplineDetailCriteria criteria, IList<DisciplineDetailModel> disciplines)
        {
            string templatePath = HttpContext.Current.Server.MapPath(TemplatePath);
            string reportName = Labels.DisciplineDetailReportName;
            string reportTitle;
            string startDate = criteria.FromDate.ToString(Constants.DateFormat);
            string endDate = criteria.ToDate.ToString(Constants.DateFormat);

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

            var rowType = (discipline.IsWeekend) ? WeekendRow : BodyRow;

            writer.WriteRow(rowType, record);
        }

        private object CreateRecord(DisciplineDetailModel discipline, ref int rowIndex)
        {
            decimal systemSubtractScore = discipline.SystemSubtractScore > 0 ? -discipline.SystemSubtractScore : 0;
            decimal approverSubtractScore = discipline.ApproverSubtractScore > 0 ? -discipline.ApproverSubtractScore : 0;

            string checkIn = ReturnValueOrEmtpyString(discipline.CheckIn.HasValue, discipline.CheckIn.Value.ToString(Constants.TimeFormat));
            string checkOut = ReturnValueOrEmtpyString(discipline.CheckOut.HasValue, discipline.CheckOut.Value.ToString(Constants.TimeFormat));
            string totalHours = ReturnValueOrEmtpyString(!string.IsNullOrEmpty(discipline.TotalHours), discipline.TotalHours);
            string missingCheckInOut = ReturnValueOrEmtpyString(discipline.MissingCheckInOut.Value > 0, discipline.MissingCheckInOut.Value.ToString());
            string workingUnderTime = ReturnValueOrEmtpyString(discipline.WorkingUnderTime.Value > 0, "1");
            string lateIn = ReturnValueOrEmtpyString(!string.IsNullOrEmpty(discipline.MinutesLateIn), discipline.MinutesLateIn);
            string earlyOut = ReturnValueOrEmtpyString(!string.IsNullOrEmpty(discipline.MinutesEarlyOut), discipline.MinutesEarlyOut);
            string absent = ReturnValueOrEmtpyString(discipline.Absent.Value > 0, discipline.Absent.Value.ToString());
            string unpaidLeave = ReturnValueOrEmtpyString(discipline.UnpaidLeave.Value > 0, discipline.UnpaidLeave.Value.ToString());
            string unpaidLeaveAbsent = ReturnValueOrEmtpyString(discipline.UnpaidLeaveAbsent.Value > 0, discipline.UnpaidLeaveAbsent.Value.ToString());
            string changeShift = ReturnValueOrEmtpyString(discipline.ChangeShift.Value > 0, discipline.ChangeShift.Value.ToString());
            string unplannedLeave = ReturnValueOrEmtpyString(discipline.UnplannedLeave.Value > 0, discipline.UnplannedLeave.Value.ToString());

            var record = new
            {
                Shift = discipline.Employee.Code,
                Nick = discipline.Employee.NickName,
                FullName = discipline.Employee.FullName,
                Category = discipline.Employee.IsLocal ? Labels.Local : Labels.Expat,
                Section = discipline.Employee.GroupName,
                Date = discipline.WorkDate,
                ShiftName = discipline.ShiftName,
                CheckIn = checkIn,
                CheckOut = checkOut,
                TotalHours = totalHours,
                MissingCheckInOut = missingCheckInOut,
                WorkingUnderTime = workingUnderTime,
                LateIn = lateIn,
                EarlyOut = earlyOut,
                Absent = absent,
                UnpaidLeave = unpaidLeave,
                UnpaidLeaveAbsent = unpaidLeaveAbsent,
                ChangeShift = changeShift,
                UnplannedLeave = unplannedLeave,
                SystemSubtractScore = systemSubtractScore,
                ApproverSubtractScore = approverSubtractScore,
                ApproverComment = discipline.ApproverComment,
                EmployeeComment = discipline.EmployeeComment
            };
            rowIndex++;

            return record;
        }

        private static string ReturnValueOrEmtpyString(bool condition, string value)
        {
            return condition ? value : string.Empty;
        }
    }
}