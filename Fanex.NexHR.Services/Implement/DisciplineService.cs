namespace Fanex.NexHR.Services.Implement
{
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
    }
}