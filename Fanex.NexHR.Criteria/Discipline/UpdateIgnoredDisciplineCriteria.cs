namespace Fanex.NexHR.Criteria.Discipline
{
    using Core;
    using Data.Repository;
    using Models.Discipline;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class UpdateIgnoredDisciplineCriteria : CriteriaBase
    {
        private const string DisciplineTemp =
            @"<Discipline WorkDate='{0}' EmployeeId='{1}' IsIgnoredChangeShift='{2}' IsIgnoredMissingCheckInOut='{3}' IsIgnoredLateIn='{4}'
                IsIgnoredEarlyOut='{5}' IsIgnoredAbsent='{6}' IsIgnoredUnplannedLeave='{7}' IsIgnoredUnpaidLeave='{8}'
                IsIgnoredWorkingUnderTime='{9}' IsIgnoredUnpaidLeaveAbsent='{10}' IsIgnoredWeekendLeave='{11}'
                IsConfirmed='{12}' ApproverSubtractScore='{13}' IsOutOfControlReason='{14}'></Discipline>";

        public int ModifiedBy { get; set; }

        public string IgnoredDetails => ConvertIgnoredDisciplineToXml();

        public bool HasUpdateOutOfControlReason { get; set; }

        [SpParam(Ignored = true)]
        public IList<IgnoredDisciplineModel> IgnoredDisciplines { get; set; }

        public override string GetSettingKey() => "EmployeeDiscipline_UpdateIgnored";

        public override bool IsValid() => !string.IsNullOrWhiteSpace(IgnoredDetails);

        private string ConvertIgnoredDisciplineToXml()
        {
            var disciplineXmlBuilder = new StringBuilder();

            if (IgnoredDisciplines != null && IgnoredDisciplines.Any())
            {
                disciplineXmlBuilder.Append(Constants.BeginRootTag);

                foreach (IgnoredDisciplineModel item in IgnoredDisciplines)
                {
                    disciplineXmlBuilder.AppendFormat(
                        DisciplineTemp,
                        item.WorkDate,
                        item.EmployeeId,
                        item.IsIgnoredChangeShift,
                        item.IsIgnoredMissingCheckInOut,
                        item.IsIgnoredLateIn,
                        item.IsIgnoredEarlyOut,
                        item.IsIgnoredAbsent,
                        item.IsIgnoredUnplannedLeave,
                        item.IsIgnoredUnpaidLeave,
                        item.IsIgnoredWorkingUnderTime,
                        item.IsIgnoredUnpaidLeaveAbsent,
                        item.IsIgnoredWeekendLeave,
                        item.IsConfirmed,
                        item.ApproverSubtractScore,
                        item.IsOutOfControlReason);
                }

                disciplineXmlBuilder.Append(Constants.EndRootTag);
            }

            return disciplineXmlBuilder.ToString();
        }
    }
}