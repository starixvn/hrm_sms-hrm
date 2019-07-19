namespace Fanex.NexHR.Repositories
{
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using Fanex.Common.Extensions;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.NexHR.Criteria.Request;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.Models.Supervisor;

    public class RequestRepository : IRequestRepository
    {
        public RequestEntity GetRequestById(ICriteria criteria)
        {
            IList<RequestEntity> entityList = null;
            using (ObjectDb db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (IDataReader reader = db.ExecuteReader(criteria.GetSpParams()))
                {
                    if (reader != null)
                    {
                        entityList = FillData(reader);
                    }
                }
            }

            return entityList.FirstOrDefault();
        }

        public IList<RequestEntity> GetHistoryRequest(ICriteria criteria)
        {
            IList<RequestEntity> entityList = null;
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (IDataReader reader = db.ExecuteReader(criteria.GetSpParams()))
                {
                    if (reader != null)
                    {
                        entityList = FillData(reader);
                    }
                }
            }

            return entityList;
        }

        public IList<RequestEntity> GetRequests(ICriteria criteria)
        {
            IList<RequestEntity> entityList = new List<RequestEntity>();
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (IDataReader reader = db.ExecuteReader(criteria.GetSpParams()))
                {
                    if (reader != null)
                    {
                        entityList = FillData(reader);
                    }
                }
            }

            return entityList;
        }

        public RequestRelatedPeopleModel GetRequestRelatedPeople(GetRequestRelatedPeopleCriteria criteria)
        {
            RequestRelatedPeopleModel relatedPeople = null;
            using (IObjectDb db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (var multi = db.QueryMultiple(criteria))
                {
                    relatedPeople = new RequestRelatedPeopleModel();
                    relatedPeople.Requester = multi.Read<SupervisorNameModel>().FirstOrDefault();
                    relatedPeople.Verifier = multi.Read<SupervisorNameModel>().FirstOrDefault();
                    relatedPeople.Approver = multi.Read<SupervisorNameModel>().FirstOrDefault();
                    relatedPeople.ListHR = multi.Read<SupervisorNameModel>().ToList();
                    relatedPeople.EmailSetting = multi.Read<EmailSettingsModel>().FirstOrDefault();
                }
            }

            return relatedPeople;
        }

        public decimal GetNumberRequestInMonth(GetNumberRequestInMonthCriteria criteria)
        {
            decimal numberDay = 0;
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                numberDay = (decimal)db.ExecuteScalar(criteria.GetSpParams());
            }

            return numberDay;
        }

        public VerifierAndApproverForRequest GetVerifierAndApprover(ICriteria criteria)
        {
            VerifierAndApproverForRequest supervisor = null;
            using (IObjectDb db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (var multi = db.QueryMultiple(criteria))
                {
                    supervisor = new VerifierAndApproverForRequest
                    {
                        Verifiers = multi.Read<SupervisorNameModel>().ToList(),
                        Approvers = multi.Read<SupervisorNameModel>().ToList()
                    };
                }
            }

            return supervisor;
        }

        public RequestEntity GetRequestToIgnoreById(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                return db.QueryEntity<RequestEntity, Shift, Employee, RequestTypeModel, RequestEntity>(
                    (request, shift, requester, type) =>
                    {
                        request.RequestType = type;
                        request.FromShift = shift;
                        request.Requester = requester;

                        return request;
                    },
                    "ShiftId, RequesterId, RequestType",
                    criteria.GetSpParams());
            }
        }

        public IList<LeaveDisciplineModel> GetLeaveRequestToIgnoreById(ICriteria criteria)
        {
            IList<LeaveDisciplineModel> disciplines = null;
            using (IObjectDb db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (var multi = db.QueryMultiple(criteria))
                {
                    IList<LeaveDisciplineDetailModel> leaveDetails;
                    disciplines = multi.Read<LeaveDisciplineModel>().ToList();
                    leaveDetails = multi.Read<LeaveDisciplineDetailModel>().ToList();
                    foreach (var discipline in disciplines)
                    {
                        discipline.LeaveDetail = leaveDetails.Where(t => t.WorkDate == discipline.WorkDate).ToList();
                    }
                }
            }

            return disciplines;
        }

        private IList<RequestEntity> FillData(IDataReader reader)
        {
            IList<RequestEntity> entityList = new List<RequestEntity>();
            while (reader.Read())
            {
                RequestEntity entity = FillDataEntity(reader);
                entityList.Add(entity);
            }

            return entityList;
        }

        private RequestEntity FillDataEntity(IDataReader reader)
        {
            var entity = new RequestEntity
            {
                Id = reader.ToInt("RequestId"),
                RequestType = new RequestTypeModel { Id = reader.ToShort("RequestType") },
                Status = new RequestStatus { Id = reader.ToByte("Status") },
                FromDate = reader.ToDateTime("FromDate"),
                ToDate = reader.ToDateTime("ToDate"),
                TotalDays = reader.ToDecimal("TotalDays"),
                IsHalfDay = reader.ToBool("IsHalfDay"),
                FromTime = reader.ToDateTime("FromTime"),
                ToTime = reader.ToDateTime("ToTime"),
                KeepInViewType = reader.ToByte("KeepInViewType"),
                LeaveCountDetails = reader.ToString("LeaveCountDetails"),
                LeaveType = new LeaveType
                {
                    Id = reader.ToInt("LeaveTypeId"),
                    Name = reader.ToString("LeaveTypeName")
                },
                Remark = reader.ToString("Remark"),
                TotalDaysRequestInMonth = reader.ToDecimal("TotalDaysRequestInMonth"),
                CreatedDate = reader.ToDateTime("CreatedDate"),
                RefNO = reader.ToString("RefNo"),
                ApprovedDate = reader.ToDateTime("ApprovedDate"),
                VerifiedDate = reader.ToDateTime("VerifiedDate"),
                ProcessedDate = reader.ToDateTime("ProcessedDate"),
                Requester = GetRequester(reader),
                ModifiedBy = new Employee
                {
                    Id = reader.ToInt("ModifiedById"),
                    NickName = reader.ToString("ModifiedByNickName"),
                    FullName = reader.ToString("ModifiedByFullName")
                },
                VerifiedBy = new Employee
                {
                    Id = reader.ToInt("VerifierId"),
                    NickName = reader.ToString("VerifierNickName"),
                    FullName = reader.ToString("VerifierFullName"),
                },
                ApprovedBy = new Employee
                {
                    Id = reader.ToInt("ApproverId"),
                    NickName = reader.ToString("ApproverNickName"),
                    FullName = reader.ToString("ApproverFullName"),
                },
                ProcessedBy = new Employee
                {
                    Id = reader.ToInt("ProcessedId"),
                    NickName = reader.ToString("ProcessedNickName"),
                    FullName = reader.ToString("ProcessedFullName"),
                },
                FromShift = new Shift
                {
                    Id = reader.ToInt("FromShiftId"),
                    Name = reader.ToString("FromShiftName")
                },
                RequestedDate = reader.ToDateTime("RequestedDate"),
                TotalMins = reader.ToInt("TotalMinutes"),
                ActualLateInEarlyOut = reader.ToInt("ActualInOutMins"),
                CrossDay = reader.ToDateTime("FromTime").DayOfYear > reader.ToDateTime("ToTime").DayOfYear,
                ToShift = new Shift
                {
                    Id = reader.ToInt("ToShiftId"),
                    Name = reader.ToString("ToShiftName")
                },
                IsWarning = reader.ToBool("IsWarning"),
                ModifiedDate = reader.ToDateTime("ModifiedDate"),
                VerifiedStatus = reader.ToInt("VerifiedStatus"),
                ApprovedStatus = reader.ToInt("ApprovedStatus"),
                ProcessedStatus = reader.ToInt("ProcessedStatus"),
                ReworkCount = reader.ToInt("ReworkCount")
            };

            return entity;
        }

        private Employee GetRequester(IDataReader reader)
        {
            return new Employee
            {
                Id = reader.ToInt("RequesterId"),
                NickName = reader.ToString("RequesterNickName"),
                FullName = reader.ToString("RequesterFullName"),
                AnnualLeaveBalance = reader.ToDecimal("AnnualLeaveBalance"),
                Team = new Team
                {
                    Id = reader.ToInt("GroupId"),
                    Code = reader.ToString("GroupCode"),
                    Name = reader.ToString("GroupName")
                }
            };
        }
    }
}