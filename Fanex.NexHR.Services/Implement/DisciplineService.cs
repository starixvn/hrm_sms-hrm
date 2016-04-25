namespace Fanex.NexHR.Services.Implement
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Criteria.Discipline;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Discipline;
    using Fanex.NexHR.Repositories.Implement;
    using Fanex.NexHR.Repositories.Interfaces;
    using Fanex.NexHR.Resources;
    using Fanex.NexHR.Services.Interfaces;

    public class DisciplineService : BaseService, IDisciplineService
    {
        private readonly IDisciplineRepository _disciplineRepository;

        public DisciplineService()
            : this(null)
        {
        }

        public DisciplineService(IDisciplineRepository disciplineRepository)
        {
            _disciplineRepository = disciplineRepository ?? new DisciplineRepository();
        }

        public IList<DisciplineSummaryModel> GetDisciplineSummary(GetDisciplineSummaryCriteria criteria)
        {
            IList<DisciplineSummaryModel> disciplines = new List<DisciplineSummaryModel>();

            if (criteria != null && criteria.IsValid())
            {
                disciplines = _disciplineRepository.GetDisciplineSummary(criteria);
                disciplines = disciplines.OrderByDescending(x => x.Permission).ToList();
            }

            return disciplines;
        }

        public IList<DisciplineDetailModel> GetDisciplineDetail(GetDisciplineDetailCriteria criteria)
        {
            IList<DisciplineDetailModel> disciplines = new List<DisciplineDetailModel>();

            if (criteria != null && criteria.IsValid())
            {
                disciplines = _disciplineRepository.GetDisciplineDetail(criteria);
            }

            return disciplines;
        }

        public IList<OtherViolationModel> GetOtherViolation(GetOtherViolationOfEmployeeCriteria criteria)
        {
            IList<OtherViolationModel> violations = new List<OtherViolationModel>();

            if (criteria != null && criteria.IsValid())
            {
                violations = GetData<OtherViolationModel>(criteria).ToList();
            }

            return violations;
        }

        public IList<DisciplineTypeModel> GetDisciplineType(GetDisciplineTypeCriteria criteria)
        {
            IList<DisciplineTypeModel> disciplineTypes = new List<DisciplineTypeModel>();

            if (criteria != null && criteria.IsValid())
            {
                disciplineTypes = GetData<DisciplineTypeModel>(criteria).ToList();
            }

            return disciplineTypes;
        }

        public IList<DisciplineTypeModel> GetOtherViolationType(GetOtherViolationTypeCriteria criteria)
        {
            IList<DisciplineTypeModel> otherViolationTypes = new List<DisciplineTypeModel>();

            if (criteria != null && criteria.IsValid())
            {
                otherViolationTypes = GetData<DisciplineTypeModel>(criteria).ToList();
            }

            return otherViolationTypes;
        }

        public CommandResult InsertOtherViolation(InsertOtherViolationCriteria criteria)
        {
            CommandResult commandResult = new CommandResult();

            if (criteria != null && criteria.IsValid())
            {
                commandResult = Repository.Insert(criteria);
            }

            return commandResult;
        }

        public CommandResult DeleteOtherViolation(DeleteOtherViolationCriteria criteria)
        {
            CommandResult commandResult = new CommandResult();

            if (criteria != null && criteria.IsValid())
            {
                commandResult = Repository.Delete(criteria);
            }

            return commandResult;
        }

        public void ExportDisciplineSummaryReport(
            GetDisciplineSummaryCriteria criteria,
            IList<DisciplineSummaryModel> disciplines,
            string templatePath)
        {
            if (criteria != null)
            {
                string reportName = Labels.DisciplineSummaryReportName,
                    reportTitle,
                    startDate = criteria.FromDate.ToString(Constants.DateFormat),
                    endDate = criteria.ToDate.ToString(Constants.DateFormat);

                if (criteria.EmployeeId != null)
                {
                    reportTitle = string.Format(Labels.DisciplineSummaryReportTitle, criteria.EmployeeName, startDate, endDate);
                }
                else if (criteria.GroupId != null)
                {
                    reportTitle = string.Format(Labels.DisciplineSummaryReportTitle, criteria.GroupName, startDate, endDate);
                }
                else
                {
                    reportTitle = string.Format(Labels.DisciplineSummaryReportTitleNoPrefix, startDate, endDate);
                }

                var writer = new ExcelTemplateWriter(templatePath, reportName, Labels.DisciplineSummaryReportSheet);

                // Fill report title.
                writer.WriteRow(0, new object[] { reportTitle });
                writer.CopyRows(1, 1);

                // Fill report header.
                writer.CopyRows(3, 1);

                FillContentRowDisciplineSummaryReport(disciplines, ref writer);
                writer.SyncColumnsWidth(17);

                writer.Render();
            }
        }

        public void ExportDisciplineDetailReport(
            GetDisciplineDetailCriteria criteria,
            IList<DisciplineDetailModel> disciplineDetails,
            string templatePath)
        {
            if (criteria != null)
            {
                string reportName = Labels.DisciplineDetailReportName,
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
                writer.CopyRows(3, 1);

                FillContentRowDisciplineDetailReport(disciplineDetails, ref writer);
                writer.SyncColumnsWidth(20);

                writer.Render();
            }
        }

        private void FillContentRowDisciplineSummaryReport(IList<DisciplineSummaryModel> disciplines, ref ExcelTemplateWriter writer)
        {
            int rowIndex = 1;

            foreach (DisciplineSummaryModel discipline in disciplines)
            {
                FillRowDisciplineSummaryReport(discipline, ref writer, ref rowIndex);
            }
        }

        private void FillRowDisciplineSummaryReport(DisciplineSummaryModel discipline, ref ExcelTemplateWriter writer, ref int rowIndex)
        {
            object record = CreateRecordDisciplineSummaryReport(discipline, ref rowIndex);
            writer.WriteRow(4, record);
        }

        private object CreateRecordDisciplineSummaryReport(DisciplineSummaryModel discipline, ref int rowIndex)
        {
            float otherViolations = discipline.OtherViolationsScore > 0 ? -discipline.OtherViolationsScore : 0,
                systemSubtractScore = discipline.SystemSubtractScore > 0 ? -discipline.SystemSubtractScore : 0,
                approverSubtractScore = discipline.ApproverSubtractScore > 0 ? -discipline.ApproverSubtractScore : 0;

            var record = new
            {
                Count = rowIndex,
                Section = discipline.Employee.GroupName,
                Code = discipline.Employee.Code,
                Nick = discipline.Employee.NickName,
                FullName = discipline.Employee.FullName,
                Category = discipline.Employee.IsLocal ? Labels.Local : Labels.Expat,
                MissingCheckInOut = discipline.MissingCheckInOut,
                WorkingUnderTime = discipline.WorkingUnderTime,
                LateIn = discipline.LateIn,
                EarlyOut = discipline.EarlyOut,
                Absent = discipline.Absent,
                UnpaidLeave = discipline.UnpaidLeave,
                UnpaidLeaveAbsent = discipline.UnpaidLeaveAbsent,
                ChangeShift = discipline.ChangeShift,
                OtherViolation = (decimal)otherViolations,
                SystemSubtractScore = (decimal)systemSubtractScore,
                ApproverSubtractScore = (decimal)approverSubtractScore
            };
            rowIndex++;

            return record;
        }

        private void FillContentRowDisciplineDetailReport(IList<DisciplineDetailModel> disciplineDetails, ref ExcelTemplateWriter writer)
        {
            int rowIndex = 1;

            foreach (DisciplineDetailModel disciplineDetail in disciplineDetails)
            {
                FillRowDisciplineDetailReport(disciplineDetail, ref writer, ref rowIndex);
            }
        }

        private void FillRowDisciplineDetailReport(DisciplineDetailModel disciplineDetail, ref ExcelTemplateWriter writer, ref int rowIndex)
        {
            object record = CreateRecordDisciplineDetailReport(disciplineDetail, ref rowIndex);
            writer.WriteRow(4, record);
        }

        private object CreateRecordDisciplineDetailReport(DisciplineDetailModel disciplineDetail, ref int rowIndex)
        {
            decimal systemSubtractScore = disciplineDetail.SystemSubtractScore > 0 ? -disciplineDetail.SystemSubtractScore : 0,
                approverSubtractScore = disciplineDetail.ApproverSubtractScore > 0 ? -disciplineDetail.ApproverSubtractScore : 0;
            float totalHours = !string.IsNullOrEmpty(disciplineDetail.TotalHours.Value) ? float.Parse(disciplineDetail.TotalHours.Value) * 60 : 0;
            string checkIn = disciplineDetail.CheckIn.HasValue ? disciplineDetail.CheckIn.Value.ToString(Constants.TimeFormat) : string.Empty,
                checkOut = disciplineDetail.CheckOut.HasValue ? disciplineDetail.CheckOut.Value.ToString(Constants.TimeFormat) : string.Empty,
                missingCheckInOut = disciplineDetail.MissingCheckInOut.Value,
                workingUnderTime = disciplineDetail.TotalHours.Value,
                lateIn = disciplineDetail.LateIn.Value,
                earlyOut = disciplineDetail.EarlyOut.Value,
                absent = disciplineDetail.Absent.Value,
                unpaidLeave = disciplineDetail.UnpaidLeave.Value,
                unpaidLeaveAbsent = disciplineDetail.UnpaidLeaveAbsent.Value,
                changeShift = disciplineDetail.ChangeShift.Value;

            var record = new
            {
                Shift = disciplineDetail.Employee.Code,
                Nick = disciplineDetail.Employee.NickName,
                FullName = disciplineDetail.Employee.FullName,
                Category = disciplineDetail.Employee.IsLocal ? Labels.Local : Labels.Expat,
                Section = disciplineDetail.Employee.GroupName,
                Date = disciplineDetail.WorkDate,
                Code = disciplineDetail.ShiftName,
                CheckIn = checkIn,
                CheckOut = checkOut,
                TotalHours = totalHours > 0 ? ConvertNumberToTime(totalHours) : string.Empty,
                MissingCheckInOut = !string.IsNullOrEmpty(missingCheckInOut) && !missingCheckInOut.Equals("0") ? missingCheckInOut : string.Empty,
                WorkingUnderTime = !string.IsNullOrEmpty(workingUnderTime) && !workingUnderTime.Equals("0") && disciplineDetail.TotalHours.HasViolated ? "1" : string.Empty,
                LateIn = !string.IsNullOrEmpty(lateIn) && !lateIn.Equals("0") ? ConvertNumberToTime(double.Parse(lateIn)) : string.Empty,
                EarlyOut = !string.IsNullOrEmpty(earlyOut) && !earlyOut.Equals("0") ? ConvertNumberToTime(double.Parse(earlyOut)) : string.Empty,
                Absent = !string.IsNullOrEmpty(absent) && !absent.Equals("0") ? absent : string.Empty,
                UnpaidLeave = !string.IsNullOrEmpty(unpaidLeave) && !unpaidLeave.Equals("0") ? unpaidLeave : string.Empty,
                UnpaidLeaveAbsent = !string.IsNullOrEmpty(unpaidLeaveAbsent) && !unpaidLeaveAbsent.Equals("0") ? unpaidLeaveAbsent : string.Empty,
                ChangeShift = !string.IsNullOrEmpty(changeShift) && !changeShift.Equals("0") ? changeShift : string.Empty,
                SystemSubtractScore = systemSubtractScore,
                ApproverSubtractScore = approverSubtractScore,
                ApproverComment = disciplineDetail.ApproverComment,
                EmployeeComment = disciplineDetail.EmployeeComment
            };
            rowIndex++;

            return record;
        }

        private string ConvertNumberToTime(double number)
        {
            double numberMinutes = Math.Round(number);

            var timeSpan = new TimeSpan(0, (int)numberMinutes, 0);
            string time = timeSpan.ToString(@"hh\:mm");

            return time;
        }
    }
}