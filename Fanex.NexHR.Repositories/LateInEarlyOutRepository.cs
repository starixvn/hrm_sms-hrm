// --------------------------------------------------------------------------
// <copyright file="LateInEarlyOutRepository.cs" company="Nexcel Solutions Vietnam">
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
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Fanex.Common.Extensions;
    using Fanex.Data;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.Repositories.Interfaces;

    /// <summary>
    /// Late In Early Out Repository.
    /// </summary>
    public class LateInEarlyOutRepository : ILateInEarlyOutRepository
    {
        /// <summary>
        /// Insert Late in - Early out request.
        /// </summary>
        /// <param name="empId">Employee Id.</param>
        /// <param name="requestType">Request Type.</param>
        /// <param name="remark">Description for this request.</param>
        /// <param name="createdDate">Created date.</param>
        /// <param name="createdBy">Created by.</param>
        /// <param name="verifierId">Verifier's Id.</param>
        /// <param name="approverId">Approver's Id.</param>
        /// <param name="errorCode">Error code.</param>
        /// <param name="message">Error message.</param>
        /// <param name="requestDate">Request's date.</param>
        /// <param name="lateInMins">Late in minutes.</param>
        /// <param name="earlyOutMins">Early out minutes.</param>
        /// <returns>Return request id.</returns>
        public long InsertLateInEarlyOutRequest(
            int empId,
            short requestType,
            string remark,
            DateTime createdDate,
            int createdBy,
            int? verifierId,
            int? approverId,
            out int errorCode,
            out string message,
            DateTime requestDate,
            int? lateInMins = null,
            int? earlyOutMins = null)
        {
            using (var db = new ObjectDb("WorkingTimeRequest_Insert_LateInEarlyOutRequest"))
            {
                db.AddInParameter("@EmpId", DbType.Int32, empId);
                db.AddInParameter("@RequestType", DbType.Int16, requestType);
                db.AddInParameter("@Remark", DbType.String, remark);
                db.AddInParameter("@OnDate", DbType.DateTime, requestDate);
                db.AddInParameter("@LateInMins", DbType.Int32, lateInMins);
                db.AddInParameter("@EarlyOutMins", DbType.Int32, earlyOutMins);
                db.AddInParameter("@CreatedDate", DbType.DateTime, createdDate);
                db.AddInParameter("@CreatedBy", DbType.Int32, createdBy);
                db.AddInParameter("@VerifierId", DbType.Int32, verifierId);
                db.AddInParameter("@ApproverId", DbType.Int32, approverId);
                db.AddInParameter("@HRId", DbType.Int32, null);

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
        /// Updates the late in early out working time request.
        /// </summary>
        /// <param name="requestId">The request identifier.</param>
        /// <param name="empId">The employee identifier.</param>
        /// <param name="requestType">Type of the request.</param>
        /// <param name="remark">The remark.</param>
        /// <param name="requestDate">The on date.</param>
        /// <param name="verifierId">The verifier identifier.</param>
        /// <param name="approverId">The approver identifier.</param>
        /// <param name="modifiedDate">The modified date.</param>
        /// <param name="modifiedBy">The modified by.</param>
        /// <param name="errorCode">The error code.</param>
        /// <param name="message">The message.</param>
        /// <param name="lateInMins">The late in minutes.</param>
        /// <param name="earlyOutMins">The early out minutes.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool UpdateLateInEarlyOutRequest(
            int requestId,
            int empId,
            int? requestType,
            string remark,
            DateTime requestDate,
            int? verifierId,
            int? approverId,
            DateTime modifiedDate,
            int modifiedBy,
            out int errorCode,
            out string message,
            int? lateInMins = null,
            int? earlyOutMins = null)
        {
            using (var db = new ObjectDb("WorkingTimeRequest_Update_LateInEarlyOutRequest"))
            {
                db.AddInParameter("@RequestId", DbType.Int32, requestId);
                db.AddInParameter("@EmpId", DbType.Int32, empId);
                db.AddInParameter("@RequestType", DbType.Int16, requestType);
                db.AddInParameter("@Remark", DbType.String, remark);
                db.AddInParameter("@OnDate", DbType.DateTime, requestDate);
                db.AddInParameter("@VerifierId", DbType.Int32, verifierId);
                db.AddInParameter("@ApproverId", DbType.Int32, approverId);
                db.AddInParameter("@ModifiedDate", DbType.DateTime, modifiedDate);
                db.AddInParameter("@ModifiedBy", DbType.Int32, modifiedBy);
                db.AddInParameter("@LateinMins", DbType.Int32, lateInMins);
                db.AddInParameter("@EarlyOutMins", DbType.Int32, earlyOutMins);

                db.AddOutParameter("@ErrCode", DbType.Int32, 8);
                db.AddOutParameter("@ErrMsg", DbType.String, 200);

                var result = db.ExecuteNonQuery() != 0;
                message = db.GetParameterValue("ErrMsg") as string;
                errorCode = (int)db.GetParameterValue("ErrCode");

                return result;
            }
        }

        /// <summary>
        /// Method get late in early out request.
        /// </summary>
        /// <param name="role">Employee's role id.</param>
        /// <param name="fromDate">From date.</param>
        /// <param name="toDate">Request to date.</param>
        /// <param name="groupIds">Group's id.</param>
        /// <param name="isDailyRequest">Is daily request.</param>
        /// <param name="employeeId">Employee's id.</param>
        /// <param name="status">Request's status.</param>
        /// <param name="logInId">Log in id.</param>
        /// <param name="isLocal">Expat or Local.</param>
        /// <returns>List of RequestEntity.</returns>
        public IList<RequestEntity> GetLateInEarlyOutRequests(
            string role,
            DateTime fromDate,
            DateTime toDate,
            string groupIds,
            bool isDailyRequest = false,
            int? employeeId = null,
            byte? status = null,
            int? logInId = null,
            byte? isLocal = null)
        {
            IList<RequestEntity> entityList = new List<RequestEntity>();
            var keyName = isDailyRequest
                ? "WorkingTimeRequest_GetRequestByRequestDate_LateInEarlyOutType"
                : "WorkingTimeRequest_GetRequestByCreateDate_LateInEarlyOutType";
            using (var db = new ObjectDb(keyName))
            {
                using (IDataReader reader =
                    db.ExecuteReader(new { empid = employeeId, groupIds, fromDate, toDate, status, excludedEmpID = logInId, role = role, supervisorID = logInId, isLocal }))
                {
                    if (reader != null)
                    {
                        while (reader.Read())
                        {
                            var entity = this.FillDataForGettingLateInEarlyOutRequest(reader);
                            entityList.Add(entity);
                        }
                    }
                }
            }

            return entityList;
        }

        /// <summary>
        /// Gets the late in early out by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>Request Entity.</returns>
        public RequestEntity GetLateInEarlyOutRequestById(long id)
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
                                RequestedDate = reader.ToDateTime("FromDate"),
                                CreatedDate = reader.ToDateTime("CreatedDate"),
                                Status = new RequestStatus { Id = reader.ToByte("Status") },
                                EmpId = reader.ToInt("RequesterID"),
                                Remark = reader.ToString("Remark"),
                                TotalMins = reader.ToInt("Time"),
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
        /// Method Get Late In Early Out Request History.
        /// </summary>
        /// <param name="requestId">Request Id.</param>
        /// <param name="requestTypeId">Request Type Id.</param>
        /// <returns>List of RequestEntity.</returns>
        public IList<RequestEntity> GetHistoricLateInEarlyOutRequests(long requestId, int requestTypeId)
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
                            var entity = this.FillDataForGettingLateInEarlyOutRequest(reader);
                            entityList.Add(entity);
                        }
                    }
                }
            }

            return entityList;
        }

        /// <summary>
        /// Fills the data for getting late in early out request.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns>The object data of RequestEntity.</returns>
        private RequestEntity FillDataForGettingLateInEarlyOutRequest(IDataReader reader)
        {
            var entity = new RequestEntity
            {
                Id = reader.ToInt("RequestId"),
                Type = new RequestType { Id = reader.ToShort("RequestType") },
                FromShift = new Shift { Name = reader.ToString("Shift") },
                RequestedDate = reader.ToDateTime("FromDate"),
                FromDate = reader.ToDateTime("FromDate"),
                ToDate = reader.ToDateTime("ToDate"),
                TotalMins = reader.ToInt("Time"),
                CreatedDate = reader.ToDateTime("CreatedDate"),
                Remark = reader.ToString("Remark"),
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
