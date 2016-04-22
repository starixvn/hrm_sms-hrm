namespace Fanex.NexHR.Repositories.Implement
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.NexHR.Models.Discipline;
    using Fanex.NexHR.Repositories.Interfaces;

    public class DisciplineRepository : IDisciplineRepository
    {
        public IList<DisciplineSummaryModel> GetDisciplineSummary(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                return db.Query<DisciplineSummaryModel, EmployeeInfoModel, DisciplineSummaryModel>(
                    (discipline, employee) =>
                    {
                        discipline.Employee = employee;

                        return discipline;
                    },
                    "EmployeeId",
                    criteria.GetSpParams()).ToList();
            }
        }

        public IList<DisciplineDetailModel> GetDisciplineDetail(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                IList<dynamic> disciplines = db.Query<dynamic>(criteria.GetSpParams()).ToList();

                return MappingObjectForDisciplineDetail(disciplines);
            }
        }

        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules", "SP2101:MethodMustNotContainMoreLinesThan", Justification = "Need to review here.")]
        private static IList<DisciplineDetailModel> MappingObjectForDisciplineDetail(IList<dynamic> disciplines)
        {
            return disciplines.Select(discipline => new DisciplineDetailModel
            {
                WorkDate = discipline.WorkDate,
                ShiftName = discipline.ShiftName,
                CheckIn = discipline.CheckIn,
                CheckOut = discipline.CheckOut,
                Employee = new EmployeeInfoModel
                {
                    EmployeeId = discipline.EmployeeId,
                    NickName = discipline.NickName,
                    FullName = discipline.FullName,
                    Code = discipline.Code,
                    IsLocal = discipline.IsLocal,
                    GroupId = discipline.GroupId,
                    GroupName = discipline.GroupName,
                    GroupCode = discipline.GroupCode
                },
                ChangeShift = new ViolationModel
                {
                    ViolationId = discipline.ChangeShiftId,
                    Value = discipline.ChangeShiftValue,
                    Score = discipline.ChangeShiftScore,
                    HasViolated = discipline.ChangeShiftHasViolated,
                    HasIgnored = discipline.ChangeShiftHasIgnored,
                    HasApprovedBy = discipline.ChangeShiftHasApprovedBy
                },
                MissingCheckInOut = new ViolationModel
                {
                    ViolationId = discipline.MissingCheckInOutId,
                    Value = discipline.MissingCheckInOutValue,
                    Score = discipline.MissingCheckInOutScore,
                    HasViolated = discipline.MissingCheckInOutHasViolated,
                    HasIgnored = discipline.MissingCheckInOutHasIgnored,
                    HasApprovedBy = discipline.MissingCheckInOutHasApprovedBy
                },
                LateIn = new ViolationModel
                {
                    ViolationId = discipline.LateInId,
                    Value = discipline.LateInValue,
                    Score = discipline.LateInScore,
                    HasViolated = discipline.LateInHasViolated,
                    HasIgnored = discipline.LateInHasIgnored,
                    HasApprovedBy = discipline.LateInHasApprovedBy
                },
                EarlyOut = new ViolationModel
                {
                    ViolationId = discipline.EarlyOutId,
                    Value = discipline.EarlyOutValue,
                    Score = discipline.EarlyOutScore,
                    HasViolated = discipline.EarlyOutHasViolated,
                    HasIgnored = discipline.EarlyOutHasIgnored,
                    HasApprovedBy = discipline.EarlyOutHasApprovedBy
                },
                Absent = new ViolationModel
                {
                    ViolationId = discipline.AbsentId,
                    Value = discipline.AbsentValue,
                    Score = discipline.AbsentScore,
                    HasViolated = discipline.AbsentHasViolated,
                    HasIgnored = discipline.AbsentHasIgnored,
                    HasApprovedBy = discipline.AbsentHasApprovedBy
                },
                UnpaidLeave = new ViolationModel
                {
                    ViolationId = discipline.UnpaidLeaveId,
                    Value = discipline.UnpaidLeaveValue,
                    Score = discipline.UnpaidLeaveScore,
                    HasViolated = discipline.UnpaidLeaveHasViolated,
                    HasIgnored = discipline.UnpaidLeaveHasIgnored,
                    HasApprovedBy = discipline.UnpaidLeaveHasApprovedBy
                },
                TotalHours = new ViolationModel
                {
                    ViolationId = discipline.TotalHoursId,
                    Value = discipline.TotalHoursValue,
                    Score = discipline.TotalHoursScore,
                    HasViolated = discipline.TotalHoursHasViolated,
                    HasIgnored = discipline.TotalHoursHasIgnored,
                    HasApprovedBy = discipline.TotalHoursHasApprovedBy
                },
                UnpaidLeaveAbsent = new ViolationModel
                {
                    ViolationId = discipline.UnpaidLeaveAbsentId,
                    Value = discipline.UnpaidLeaveAbsentValue,
                    Score = discipline.UnpaidLeaveAbsentScore,
                    HasViolated = discipline.UnpaidLeaveAbsentHasViolated,
                    HasIgnored = discipline.UnpaidLeaveAbsentHasIgnored,
                    HasApprovedBy = discipline.UnpaidLeaveAbsentHasApprovedBy
                }
            }).ToList();
        }
    }
}