// --------------------------------------------------------------------------
// <copyright file="RequestRepository.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.9.9">Created</change>
// </history>

namespace Fanex.NexHR.Repositories
{
    using System.Collections.Generic;
    using System.Data;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Fanex.Common.Extensions;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.NexHR.Criteria.Request;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.Models.Supervisor;
    using Fanex.NexHR.Repositories.Interfaces;

    /// <summary>
    /// Request Repository.
    /// </summary>
    public class RequestRepository : IRequestRepository
    {
        /// <summary>
        /// Function to get request by requestId.
        /// </summary>
        /// <param name="criteria">The Criteria.</param>
        /// <returns>Request Entity.</returns>
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

        /// <summary>
        /// Function to get History request.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>List Request Entity.</returns>
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

        /// <summary>
        /// Method Get all request.
        /// </summary>
        /// <param name="criteria">GetRequest Criteria.</param>
        /// <returns>List of RequestEntity.</returns>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules", "SP2101:MethodMustNotContainMoreLinesThan", Justification = "Reviewed. Suppression is OK here.")]
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
                        ////while (reader.Read())
                        ////{
                        ////    var entity = new RequestEntity
                        ////    {
                        ////        Id = reader.ToInt("RequestId"),
                        ////        CreatedDate = reader.ToDateTime("CreatedDate"),
                        ////        Status = new RequestStatus { Id = reader.ToByte("Status") },
                        ////        RequestType = new RequestTypeModel { Id = reader.ToShort("RequestType") },
                        ////        TotalDaysRequestInMonth = reader.ToDecimal("TotalDaysRequestInMonth"),
                        ////        RequestedDate = reader.ToDateTime("RequestDate"),
                        ////        IsWarning = reader.ToBool("IsWarning"),
                        ////        Requester = new Employee
                        ////        {
                        ////            Id = reader.To<int>("RequesterID"),
                        ////            NickName = reader.To<string>("Requester"),
                        ////            FullName = reader.To<string>("FullNameRequester"),
                        ////            Team = new Team
                        ////            {
                        ////                Name = reader.To<string>("Team"),
                        ////                Code = reader.To<string>("TeamCode")
                        ////            }
                        ////        },
                        ////        ModifiedBy = new Employee
                        ////        {
                        ////            Id = reader.To<int>("ModifiedByID"),
                        ////            NickName = reader.To<string>("ModifiedBy"),
                        ////            FullName = reader.To<string>("FullNameModified"),
                        ////        },
                        ////        VerifiedBy = new Employee
                        ////        {
                        ////            Id = reader.To<int>("VerifierID"),
                        ////            NickName = reader.To<string>("VerifierBy"),
                        ////            FullName = reader.To<string>("FullNameVerifierBy"),
                        ////        },
                        ////        ApprovedBy = new Employee
                        ////        {
                        ////            Id = reader.To<int>("ApproverId"),
                        ////            NickName = reader.To<string>("ApproverBy"),
                        ////            FullName = reader.To<string>("FullNameApproverBy"),
                        ////        },
                        ////        ProcessedBy = new Employee
                        ////        {
                        ////            Id = reader.To<int>("HrId"),
                        ////            NickName = reader.To<string>("HrBy"),
                        ////            FullName = reader.To<string>("FullNameHrBy"),
                        ////        },
                        ////        ModifiedDate = reader.ToDateTime("ModifiedDate"),
                        ////        FromShift = new Shift { Name = reader.To<string>("FromShift") },
                        ////        ToShift = new Shift { Name = reader.To<string>("ToShift") },
                        ////        TotalMins = reader.ToInt("Time"),
                        ////        FromDate = reader.ToDateTime("FromDate"),
                        ////        ToDate = reader.ToDateTime("ToDate"),
                        ////        FromTime = reader.ToDateTime("FromTime"),
                        ////        ToTime = reader.ToDateTime("ToTime"),
                        ////        TotalDays = reader.ToDecimal("TotalDays"),
                        ////        IsHalfDay = reader.ToShort("RequestType") == 7 && reader.ToDouble("TotalDays") == 0.5,
                        ////        LeaveType = new LeaveType
                        ////        {
                        ////            Id = reader.ToInt("LeaveTypeId"),
                        ////            Name = reader.ToString("LeaveType")
                        ////        },
                        ////        Remark = reader.ToString("Remark"),
                        ////        VerifiedStatus = reader.ToInt("VerifiedStatus"),
                        ////        ApprovedStatus = reader.ToInt("ApprovedStatus"),
                        ////        ProcessedStatus = reader.ToInt("HRStatus"),
                        ////        ApprovedDate = reader.ToDateTime("ApprovedDate"),
                        ////        VerifiedDate = reader.ToDateTime("VerifiedDate"),
                        ////        ProcessedDate = reader.ToDateTime("HrDate"),
                        ////        RefNO = reader.ToString("RefNo")
                        ////    };
                        ////    entityList.Add(entity);
                        ////}
                    }
                }
            }

            return entityList;
        }

        /// <summary>
        /// Function to get related people of a request.
        /// </summary>
        /// <param name="criteria">GetRequestRelatedPeople Criteria.</param>
        /// <returns>RequestRelatedPeople Model.</returns>
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

        /// <summary>
        /// Method get number request in month.
        /// </summary>
        /// <param name="criteria">GetNumberRequestInMonth Criteria.</param>
        /// <returns>Number days.</returns>
        public decimal GetNumberRequestInMonth(GetNumberRequestInMonthCriteria criteria)
        {
            decimal numberDay = 0;
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                numberDay = (decimal)db.ExecuteScalar(criteria.GetSpParams());
            }

            return numberDay;
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

        /// <summary>
        /// Function to fill data for a request.
        /// </summary>
        /// <param name="reader">IData Reader.</param>
        /// <returns>Request Entity.</returns>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules", "SP2101:MethodMustNotContainMoreLinesThan", Justification = "Reviewed. Suppression is OK here.")]
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
                IsHalfDay = reader.ToDouble("TotalDays") == 0.5 && reader.ToInt("LeaveType") != 0,
                ////keep in view reader.ToBool("IsHalfDay") reader.ToShort("RequestType") == 7 && reader.ToDouble("TotalDays") == 0.5,
                FromTime = reader.ToDateTime("FromTime"),
                ToTime = reader.ToDateTime("ToTime"),
                LeaveType = new LeaveType
                {
                    Id = reader.ToInt("LeaveType"),
                    Name = reader.ToString("LeaveTypeName")
                },
                Remark = reader.ToString("Remark"),
                TotalDaysRequestInMonth = reader.ToDecimal("TotalDaysRequestInMonth"),
                CreatedDate = reader.ToDateTime("CreatedDate"),
                RefNO = reader.ToString("RefNo"),
                ApprovedDate = reader.ToDateTime("ApprovedDate"),
                VerifiedDate = reader.ToDateTime("VerifiedDate"),
                ProcessedDate = reader.ToDateTime("HrDate"),
                Requester = new Employee
                {
                    Id = reader.ToInt("RequesterId"),
                    NickName = reader.ToString("Requester"),
                    FullName = reader.ToString("FullNameRequester"),
                    Team = new Team 
                    { 
                        Name = reader.ToString("Team"), 
                        Code = reader.ToString("TeamCode") 
                    }
                },
                ModifiedBy = new Employee
                {
                    Id = reader.ToInt("ModifiedByID"),
                    NickName = reader.ToString("ModifiedBy"),
                    FullName = reader.ToString("FullNameModified")
                },
                VerifiedBy = new Employee
                {
                    Id = reader.ToInt("VerifierID"),
                    NickName = reader.ToString("VerifierBy"),
                    FullName = reader.ToString("FullNameVerifierBy"),
                },
                ApprovedBy = new Employee
                {
                    Id = reader.ToInt("ApproverID"),
                    NickName = reader.ToString("ApproverBy"),
                    FullName = reader.ToString("FullNameApproverBy"),
                },
                ProcessedBy = new Employee
                {
                    Id = reader.ToInt("HrId"),
                    NickName = reader.ToString("HrBy"),
                    FullName = reader.ToString("FullNameHrBy"),
                },
                //// Late In, Early Out
                FromShift = new Shift
                {
                    Id = reader.ToInt("ShiftId"),
                    Name = reader.ToString("ShiftName")
                },
                RequestedDate = reader.ToDateTime("RequestedDate"),
                TotalMins = reader.ToInt("Minutes"),
                //// Keep in view
                CrossDay = reader.ToDateTime("FromTime").DayOfYear > reader.ToDateTime("ToTime").DayOfYear,
                //// Change shift
                ToShift = new Shift
                {
                    Id = reader.ToInt("ToShiftId"),
                    Name = reader.ToString("ToShiftName")
                },
                IsWarning = reader.ToBool("IsWarning"),
                ModifiedDate = reader.ToDateTime("ModifiedDate"),
                VerifiedStatus = reader.ToInt("VerifiedStatus"),
                ApprovedStatus = reader.ToInt("ApprovedStatus"),
                ProcessedStatus = reader.ToInt("HRStatus"),
            };

            return entity;
        }
    }
}
