// --------------------------------------------------------------------------
// <copyright file="CheckInOutRepository.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Bradley.Mac" date="2013.11.14">Created</change>
// </history>

namespace Fanex.NexHR.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using Fanex.Common.Extensions;
    using Fanex.Data;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.Repositories.Interfaces;

    /// <summary>
    /// Check in out repository.
    /// </summary>
    public class CheckInOutRepository : RequestRepository, ICheckInOutRepository
    {
        /// <summary>
        /// Method insert check in, out request.
        /// </summary>
        /// <param name="empId">Employee Id.</param>
        /// <param name="requestType">Request Type.</param>
        /// <param name="remark">Request's description.</param>
        /// <param name="requestDate">Request's date.</param>
        /// <param name="createdDate">Created date.</param>
        /// <param name="createdBy">Created by.</param>
        /// <param name="verifierId">Verifier's id.</param>
        /// <param name="approverId">Approver's id.</param>
        /// <param name="refNo">Reference number.</param>
        /// <param name="attendantDate">Attendant date.</param>
        /// <param name="errorCode">Error code.</param>
        /// <param name="message">Error message.</param>
        /// <returns>Return request id.</returns>
        public long InsertCheckInOutRequest(
            int empId,
            int requestType,
            string remark,
            DateTime requestDate,
            DateTime createdDate,
            int createdBy,
            int? verifierId,
            int? approverId,
            string refNo,
            DateTime attendantDate,
            out int errorCode,
            out string message)
        {
            using (var db = new ObjectDb("WorkingTimeRequest_Insert_CheckInOutRequest"))
            {
                db.AddInParameter("@EmpId", DbType.Int32, empId);
                db.AddInParameter("@RequestType", DbType.Int16, requestType);
                db.AddInParameter("@Remark", DbType.String, remark);
                db.AddInParameter("@OnDate", DbType.DateTime, attendantDate);
                db.AddInParameter("@CheckTime", DbType.DateTime, requestDate);
                db.AddInParameter("@CreatedDate", DbType.DateTime, createdDate);
                db.AddInParameter("@CreatedBy", DbType.Int32, createdBy);
                db.AddInParameter("@VerifierId", DbType.Int32, verifierId);
                db.AddInParameter("@ApproverId", DbType.Int32, approverId);
                db.AddInParameter("@RefNo", DbType.String, refNo);

                db.AddOutParameter("@ErrCode", DbType.Int32, 8);
                db.AddOutParameter("@ErrMsg", DbType.String, 200);

                var result = db.ExecuteScalar();
                var requestId = result == null ? -1 : Convert.ToInt32(result);
                message = db.GetParameterValue("ErrMsg") as string;
                errorCode = (int)db.GetParameterValue("ErrCode");

                return requestId;
            }
        }

        /// <summary>
        /// Updates the check in out working time request.
        /// </summary>
        /// <param name="requestId">The request identifier.</param>
        /// <param name="empId">The Employee identifier.</param>
        /// <param name="requestType">Type of the request.</param>
        /// <param name="remark">The remark.</param>
        /// <param name="requestDate">The on date.</param>
        /// <param name="verifierId">The verifier identifier.</param>
        /// <param name="approverId">The approver identifier.</param>
        /// <param name="modifiedDate">The modified date.</param>
        /// <param name="modifiedBy">The modified by.</param>
        /// <param name="refNo">The refNo.</param>
        /// <param name="attendantDate">Attendant Date.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool UpdateCheckInOutRequest(
            int requestId,
            int empId,
            int? requestType,
            string remark,
            DateTime requestDate,
            int? verifierId,
            int? approverId,
            DateTime modifiedDate,
            int modifiedBy,
            string refNo,
            DateTime attendantDate)
        {
            using (var db = new ObjectDb("WorkingTimeRequest_Update_CheckInOutRequest"))
            {
                db.AddInParameter("@RequestId", DbType.Int32, requestId);
                db.AddInParameter("@EmpId", DbType.Int32, empId);
                db.AddInParameter("@RequestType", DbType.Int16, requestType);
                db.AddInParameter("@Remark", DbType.String, remark);
                db.AddInParameter("@OnDate", DbType.DateTime, attendantDate);
                db.AddInParameter("@CheckTime", DbType.DateTime, requestDate);
                db.AddInParameter("@RefNo", DbType.String, refNo);
                db.AddInParameter("@VerifierId", DbType.Int32, verifierId);
                db.AddInParameter("@ApproverId", DbType.Int32, approverId);

                db.AddInParameter("@ModifiedDate", DbType.DateTime, modifiedDate);
                db.AddInParameter("@ModifiedBy", DbType.Int32, modifiedBy);

                return db.ExecuteNonQuery() != 0;
            }
        }

        /// <summary>
        /// Gets the check in check out by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>Request Entity.</returns>
        public RequestEntity GetCheckInOutRequestById(long id)
        {
            IList<RequestEntity> entityList = new List<RequestEntity>();
            using (ObjectDb db = new ObjectDb(Constants.WorkingTimeRequestGetAllByRequestID))
            {
                using (IDataReader reader = db.ExecuteReader(new { RequestID = id }))
                {
                    if (reader != null)
                    {
                        while (reader.Read())
                        {
                            var entity = new RequestEntity
                            {
                                Id = reader.ToInt("RequestId"),
                                Type = new RequestType { Id = reader.ToShort("RequestType") },
                                FromShift = new Shift { Name = reader.ToString("Shift") },
                                RequestedDate = reader.ToDateTime("RequestTime"),
                                CreatedDate = reader.ToDateTime("CreatedDate"),
                                Status = new RequestStatus { Id = reader.ToByte("Status") },
                                EmpId = reader.ToInt("RequesterID"),
                                RefNO = reader.ToString("RefNo"),
                                FromDate = reader.ToDateTime("FromDate"),
                                Requester = new Employee
                                {
                                    NickName = reader.To<string>("Requester"),
                                    FullName = reader.To<string>("FullNameRequester"),
                                    Team = new Team { Name = reader.To<string>("Team"), Code = reader.To<string>("TeamCode") }
                                },
                                VerifiedBy = new Employee
                                {
                                    NickName = reader.To<string>("VerifierBy"),
                                    FullName = reader.To<string>("FullNameVerifierBy"),
                                    Id = reader.To<int>("VerifierID")
                                },
                                ApprovedBy = new Employee
                                {
                                    NickName = reader.To<string>("ApproverBy"),
                                    FullName = reader.To<string>("FullNameApproverBy"),
                                    Id = reader.To<int>("ApproverID")
                                }
                            };

                            entityList.Add(entity);
                        }
                    }
                }
            }

            return entityList.FirstOrDefault();
        }

        /// <summary>
        /// Method Get Check In Out Request History.
        /// </summary>
        /// <param name="requestId">Request Id.</param>
        /// <param name="requestTypeId">Request Type Id.</param>
        /// <returns>List of RequestEntity.</returns>
        public IList<RequestEntity> GetCheckInOutRequestHistory(long requestId, int requestTypeId)
        {
            IList<RequestEntity> entityList = new List<RequestEntity>();
            using (var db = new ObjectDb("WorkingTimeRequest_GetRequestHistory"))
            {
                using (IDataReader reader = db.ExecuteReader(new { requestid = requestId, type = requestTypeId }))
                {
                    if (reader != null)
                    {
                        while (reader.Read())
                        {
                            var entity = this.FillDataForGettingCheckInOutRequest(reader);
                            entityList.Add(entity);
                        }
                    }
                }
            }

            return entityList;
        }

        /// <summary>
        /// Method get check in out request.
        /// </summary>
        /// <param name="role">Employee's role id.</param>
        /// <param name="fromDate">From date.</param>
        /// <param name="toDate">Request to date.</param>
        /// <param name="groupIds">Group's id.</param>
        /// <param name="isDailyRequest">Is daily request.</param>
        /// <param name="empid">Employee's id.</param>
        /// <param name="status">Request's status.</param>
        /// <param name="excludedEmpID">Log in id.</param>
        /// <param name="isLocal">Expat or Local.</param>
        /// <returns>List of RequestEntity.</returns>
        public IList<RequestEntity> GetCheckInOutRequests(
            string role,
            DateTime fromDate,
            DateTime toDate,
            string groupIds,
            bool isDailyRequest = false,
            int? empid = null,
            byte? status = null,
            int? excludedEmpID = null,
            byte? isLocal = null)
        {
            IList<RequestEntity> entityList = new List<RequestEntity>();
            var keyName = isDailyRequest
                ? "WorkingTimeRequest_GetRequestByRequestDate_CheckinOutType"
                : "WorkingTimeRequest_GetRequestByCreateDate_CheckinOutType";
            using (var db = new ObjectDb(keyName))
            {
                using (IDataReader reader = db.ExecuteReader(new { empid, groupIds, fromDate, toDate, status, excludedEmpID, role, SupervisorID = excludedEmpID, isLocal }))
                {
                    if (reader != null)
                    {
                        while (reader.Read())
                        {
                            var entity = this.FillDataForGettingCheckInOutRequest(reader);
                            entityList.Add(entity);
                        }
                    }
                }
            }

            return entityList;
        }

        /// <summary>
        /// Fills the data for getting check in out request.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns>The data object of RequestEntity.</returns>
        private RequestEntity FillDataForGettingCheckInOutRequest(IDataReader reader)
        {
            var entity = new RequestEntity
            {
                Id = reader.ToInt("RequestId"),
                Type = new RequestType { Id = reader.ToShort("RequestType") },
                FromShift = new Shift { Name = reader.ToString("Shift") },
                RequestedDate = reader.ToDateTime("RequestTime"),
                CreatedDate = reader.ToDateTime("CreatedDate"),
                Status = new RequestStatus { Id = reader.ToByte("Status") },
                TotalDaysRequestInMonth = reader.ToDecimal("TotalDaysRequest"),
                ApprovedDate = reader.ToDateTime("ApprovedDate"),
                VerifiedDate = reader.ToDateTime("VerifiedDate"),
                HrDate = reader.ToDateTime("HrDate"),
                Requester = new Employee
                {
                    NickName = reader.To<string>("Requester"),
                    FullName = reader.To<string>("FullNameRequester"),
                    Team = new Team { Name = reader.To<string>("Team"), Code = reader.To<string>("TeamCode") }
                },
                ModifiedBy = new Employee
                {
                    Id = reader.To<int>("ModifiedByID"),
                    NickName = reader.To<string>("ModifiedBy"),
                    FullName = reader.To<string>("FullNameModified"),
                },
                VerifiedBy = new Employee
                {
                    Id = reader.To<int>("VerifierID"),
                    NickName = reader.To<string>("VerifierBy"),
                    FullName = reader.To<string>("FullNameVerifierBy"),
                },
                ApprovedBy = new Employee
                {
                    Id = reader.To<int>("ApproverID"),
                    NickName = reader.To<string>("ApproverBy"),
                    FullName = reader.To<string>("FullNameApproverBy"),
                },
                HrBy = new Employee
                {
                    Id = reader.To<int>("HrId"),
                    NickName = reader.To<string>("HrBy"),
                    FullName = reader.To<string>("FullNameHrBy"),
                }
            };
            return entity;
        }
    }
}
