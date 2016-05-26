namespace Fanex.NexHR.Repositories.Implement
{
    using System.Collections.Generic;
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

                IList<DisciplineDetailModel> disciplineDetails = MappingObjectForDisciplineDetail(disciplines);
                CalculateScore(disciplineDetails);

                return disciplineDetails;
            }
        }

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
                ChangeShift = GetChangeShift(discipline),
                MissingCheckInOut = GetMissingCheckInOut(discipline),
                LateIn = GetLateIn(discipline),
                EarlyOut = GetEarlyOut(discipline),
                Absent = GetAbsent(discipline),
                UnpaidLeave = GetUnpaidLeave(discipline),
                TotalHours = GetTotalHours(discipline),
                UnpaidLeaveAbsent = GetUnpaidLeaveAbsent(discipline),
                EmployeeComment = discipline.EmployeeComment,
                ApproverComment = discipline.ApproverComment
            }).ToList();
        }

        private static DisciplineModel GetChangeShift(dynamic discipline)
        {
            return new DisciplineModel
            {
                DisciplineId = discipline.ChangeShiftId,
                Value = discipline.ChangeShiftValue,
                Score = discipline.ChangeShiftScore,
                HasViolated = discipline.ChangeShiftHasViolated,
                HasIgnored = discipline.ChangeShiftHasIgnored,
                HasApprovedBy = discipline.ChangeShiftHasApprovedBy
            };
        }

        private static DisciplineModel GetMissingCheckInOut(dynamic discipline)
        {
            return new DisciplineModel
            {
                DisciplineId = discipline.MissingCheckInOutId,
                Value = discipline.MissingCheckInOutValue,
                Score = discipline.MissingCheckInOutScore,
                HasViolated = discipline.MissingCheckInOutHasViolated,
                HasIgnored = discipline.MissingCheckInOutHasIgnored,
                HasApprovedBy = discipline.MissingCheckInOutHasApprovedBy
            };
        }

        private static DisciplineModel GetLateIn(dynamic discipline)
        {
            return new DisciplineModel
            {
                DisciplineId = discipline.LateInId,
                Value = discipline.LateInValue,
                Score = discipline.LateInScore,
                HasViolated = discipline.LateInHasViolated,
                HasIgnored = discipline.LateInHasIgnored,
                HasApprovedBy = discipline.LateInHasApprovedBy
            };
        }

        private static DisciplineModel GetEarlyOut(dynamic discipline)
        {
            return new DisciplineModel
            {
                DisciplineId = discipline.EarlyOutId,
                Value = discipline.EarlyOutValue,
                Score = discipline.EarlyOutScore,
                HasViolated = discipline.EarlyOutHasViolated,
                HasIgnored = discipline.EarlyOutHasIgnored,
                HasApprovedBy = discipline.EarlyOutHasApprovedBy
            };
        }

        private static DisciplineModel GetAbsent(dynamic discipline)
        {
            return new DisciplineModel
            {
                DisciplineId = discipline.AbsentId,
                Value = discipline.AbsentValue,
                Score = discipline.AbsentScore,
                HasViolated = discipline.AbsentHasViolated,
                HasIgnored = discipline.AbsentHasIgnored,
                HasApprovedBy = discipline.AbsentHasApprovedBy
            };
        }

        private static DisciplineModel GetUnpaidLeave(dynamic discipline)
        {
            return new DisciplineModel
            {
                DisciplineId = discipline.UnpaidLeaveId,
                Value = discipline.UnpaidLeaveValue,
                Score = discipline.UnpaidLeaveScore,
                HasViolated = discipline.UnpaidLeaveHasViolated,
                HasIgnored = discipline.UnpaidLeaveHasIgnored,
                HasApprovedBy = discipline.UnpaidLeaveHasApprovedBy
            };
        }

        private static DisciplineModel GetTotalHours(dynamic discipline)
        {
            return new DisciplineModel
            {
                DisciplineId = discipline.TotalHoursId,
                Value = discipline.TotalHoursValue,
                Score = discipline.TotalHoursScore,
                HasViolated = discipline.TotalHoursHasViolated,
                HasIgnored = discipline.TotalHoursHasIgnored,
                HasApprovedBy = discipline.TotalHoursHasApprovedBy
            };
        }

        private static DisciplineModel GetUnpaidLeaveAbsent(dynamic discipline)
        {
            return new DisciplineModel
            {
                DisciplineId = discipline.UnpaidLeaveAbsentId,
                Value = discipline.UnpaidLeaveAbsentValue,
                Score = discipline.UnpaidLeaveAbsentScore,
                HasViolated = discipline.UnpaidLeaveAbsentHasViolated,
                HasIgnored = discipline.UnpaidLeaveAbsentHasIgnored,
                HasApprovedBy = discipline.UnpaidLeaveAbsentHasApprovedBy
            };
        }

        private static void CalculateScore(IList<DisciplineDetailModel> disciplineDetails)
        {
            foreach (DisciplineDetailModel disciplineDetail in disciplineDetails)
            {
                CalculateSubtractScore(disciplineDetail);
            }
        }

        private static void CalculateSubtractScore(DisciplineDetailModel disciplineDetail)
        {
            var violations = new List<DisciplineModel>
            {
                disciplineDetail.MissingCheckInOut,
                disciplineDetail.LateIn,
                disciplineDetail.EarlyOut,
                disciplineDetail.TotalHours,
                disciplineDetail.Absent,
                disciplineDetail.UnpaidLeave,
                disciplineDetail.UnpaidLeaveAbsent,
                disciplineDetail.ChangeShift
            };

            foreach (DisciplineModel violation in violations.Where(violation => violation != null && violation.HasViolated))
            {
                disciplineDetail.SystemSubtractScore += violation.Score;
                disciplineDetail.ApproverSubtractScore += !violation.HasIgnored ? violation.Score : 0;
            }
        }
    }
}