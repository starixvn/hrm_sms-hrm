// --------------------------------------------------------------------------
// <copyright file="KeepInViewRequestRepository.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Paul.Nguyen" date="2013.11.14">Created</change>
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
    /// Request Repository.
    /// </summary>
    public class KeepInViewRequestRepository : RequestRepository, IKeepInViewRequestRepository
    {
        /// <summary>
        /// Inserts the keep in view request.
        /// </summary>
        /// <param name="employeeId">The employee identifier.</param>
        /// <param name="requestType">Type of the request.</param>
        /// <param name="remark">The remark.</param>
        /// <param name="date">The on date.</param>
        /// <param name="isHalfDay">If set to <c>true</c> [is half day].</param>
        /// <param name="shiftId">The shift identifier.</param>
        /// <param name="fromTime">From time of keep in view.</param>
        /// <param name="toTime">To time of keep in view.</param>
        /// <param name="createdBy">The created by.</param>
        /// <param name="createdDate">The created date.</param>
        /// <param name="verifierId">The verifier identifier.</param>
        /// <param name="approverId">The approver identifier.</param>
        /// <param name="refNo">The reference no.</param>
        /// <param name="errorCode">The error code.</param>
        /// <param name="message">The message returned from database.</param>
        /// <returns>The request id after inserting to database.</returns>
        public long InsertKeepInViewRequest(
            int employeeId,
            short requestType,
            string remark,
            DateTime date,
            bool isHalfDay,
            int shiftId,
            DateTime? fromTime,
            DateTime? toTime,
            int createdBy,
            DateTime createdDate,
            int? verifierId,
            int? approverId,
            string refNo,
            out int errorCode,
            out string message)
        {
            using (var db = new ObjectDb(Constants.WorkingTimeRequestInsertKeepInViewRequest))
            {
                db.AddInParameter("@EmpId", DbType.Int32, employeeId);
                db.AddInParameter("@RequestType", DbType.Int16, requestType);
                db.AddInParameter("@Remark", DbType.String, remark);
                db.AddInParameter("@OnDate", DbType.DateTime, date);
                db.AddInParameter("@IsHalfDay", DbType.Boolean, isHalfDay);
                db.AddInParameter("@ShiftId", DbType.Int32, shiftId);
                db.AddInParameter("@FromTime", DbType.DateTime, fromTime);
                db.AddInParameter("@ToTime", DbType.DateTime, toTime);
                ////db.AddInParameter("@ExpExchangeDay", DbType.Decimal, null);
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
        /// Updates the keep in view request.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="employeeId">The employee identifier.</param>
        /// <param name="requestType">Type of the request.</param>
        /// <param name="remark">The remark.</param>
        /// <param name="date">The on date.</param>
        /// <param name="isHalfDay">If set to <c>true</c> [is half day].</param>
        /// <param name="shiftId">The shift identifier.</param>
        /// <param name="fromTime">From time of keep in view.</param>
        /// <param name="toTime">To time of keep in view.</param>
        /// <param name="modifiedBy">The modified by.</param>
        /// <param name="modifiedDate">The modified date.</param>
        /// <param name="verifierId">The verifier identifier.</param>
        /// <param name="approverId">The approver identifier.</param>
        /// <param name="refNo">The reference no.</param>
        /// <param name="errorCode">The error code.</param>
        /// <param name="message">The message.</param>
        /// <returns>Indicate this update action is successful or not.</returns>
        public bool UpdateKeepInViewRequest(
            long id,
            int employeeId,
            short requestType,
            string remark,
            DateTime date,
            bool isHalfDay,
            int shiftId,
            DateTime? fromTime,
            DateTime? toTime,
            int modifiedBy,
            DateTime modifiedDate,
            int? verifierId,
            int? approverId,
            string refNo,
            out int errorCode,
            out string message)
        {
            using (var db = new ObjectDb(Constants.WorkingTimeRequestUpdateKeepInViewRequest))
            {
                db.AddInParameter("@RequestId", DbType.Int64, id);
                db.AddInParameter("@EmpId", DbType.Int32, employeeId);
                db.AddInParameter("@RequestType", DbType.Int16, requestType);
                db.AddInParameter("@Remark", DbType.String, remark);
                db.AddInParameter("@OnDate", DbType.DateTime, date);
                db.AddInParameter("@IsHalfDay", DbType.Boolean, isHalfDay);
                db.AddInParameter("@ShiftId", DbType.Int32, shiftId);
                db.AddInParameter("@FromTime", DbType.DateTime, fromTime);
                db.AddInParameter("@ToTime", DbType.DateTime, toTime);
                ////db.AddInParameter("@ExpExchangeDay", DbType.Decimal, null);
                db.AddInParameter("@ModifiedDate", DbType.DateTime, modifiedDate);
                db.AddInParameter("@ModifiedBy", DbType.Int32, modifiedBy);
                db.AddInParameter("@VerifierId", DbType.Int32, verifierId);
                db.AddInParameter("@ApproverId", DbType.Int32, approverId);
                db.AddInParameter("@RefNo", DbType.String, refNo);
                db.AddOutParameter("@ErrCode", DbType.Int32, 8);
                db.AddOutParameter("@ErrMsg", DbType.String, 200);

                var result = db.ExecuteNonQuery() != 0;
                message = db.GetParameterValue("ErrMsg") as string;
                errorCode = (int)db.GetParameterValue("ErrCode");

                return result;
            }
        }

        /// <summary>
        /// Gets the keep in view request.
        /// </summary>
        /// <param name="role">The role identifier.</param>
        /// <param name="fromDate">From date of keep in view.</param>
        /// <param name="toDate">To date of keep in view.</param>
        /// <param name="groupIds">The group ids.</param>
        /// <param name="isDailyRequest">If set to <c>true</c> [is daily request].</param>
        /// <param name="employeeId">The employee identifier.</param>
        /// <param name="status">The status of keep in view.</param>
        /// <param name="logInId">The log in identifier.</param>
        /// <param name="isLocal">Expat or Local.</param>
        /// <returns>The list of keep in view requests.</returns>
        public IList<RequestEntity> GetKeepInViewRequests(
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
                ? Constants.WorkingTimeRequestGetRequestByRequestDateKeepInViewType
                : Constants.WorkingTimeRequestGetRequestByCreateDateKeepInViewType;
            using (var db = new ObjectDb(keyName))
            {
                using (IDataReader reader =
                    db.ExecuteReader(new { empid = employeeId, groupIds, fromDate, toDate, status, excludedEmpID = logInId, role = role, supervisorID = logInId, isLocal }))
                {
                    if (reader != null)
                    {
                        while (reader.Read())
                        {
                            var entity = this.FillDataForGettingKeepInViewRequest(reader);
                            entityList.Add(entity);
                        }
                    }
                }
            }

            return entityList;
        }

        /// <summary>
        /// Gets the keep in view by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>The keep in view request match with the request id.</returns>
        public RequestEntity GetKeepInViewRequestById(long id)
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
                            var entity = this.FillDataForGettingKeepInViewById(reader);
                            entityList.Add(entity);
                        }
                    }
                }
            }

            return entityList.FirstOrDefault();
        }

        /// <summary>
        /// Method GetKeepInViewRequestHistory.
        /// </summary>
        /// <param name="requestId">Request Id.</param>
        /// <param name="requestTypeId">Request type id.</param>
        /// <returns>List RequestEntity.</returns>
        public IList<RequestEntity> GetHistoricKeepInViewRequests(long requestId, int requestTypeId)
        {
            IList<RequestEntity> entityList = new List<RequestEntity>();
            using (var db = new ObjectDb(Constants.WorkingTimeRequestGetRequestHistory))
            {
                using (IDataReader reader = db.ExecuteReader(new { requestid = requestId, type = requestTypeId }))
                {
                    if (reader != null)
                    {
                        while (reader.Read())
                        {
                            var entity = this.FillDataForGettingKeepInViewRequest(reader);
                            entityList.Add(entity);
                        }
                    }
                }
            }

            return entityList;
        }

        /// <summary>
        /// Fills the data to request entity.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns>The request entity of keep in view.</returns>
        private RequestEntity FillDataForGettingKeepInViewById(IDataReader reader)
        {
            var fromTime = reader.ToDateTime("FromTime");
            var toTime = reader.ToDateTime("ToTime");

            var entity = new RequestEntity
            {
                Id = reader.ToLong("RequestId"),
                Type = new RequestType { Id = reader.ToShort("RequestType") },
                RequestedDate = reader.ToDateTime("OnDate"),
                Remark = reader.ToString("Remark"),
                Shift = reader.ToString("Shift"),
                ShiftId = reader.ToInt("ShiftId"),
                CreatedDate = reader.ToDateTime("CreatedDate"),
                Status = new RequestStatus { Id = reader.ToByte("Status") },
                EmpId = reader.ToInt("RequesterID"),
                RefNO = reader.ToString("RefNo"),
                IsHalfDay = reader.ToBool("IsHalfDay"),
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
                },
                CrossDay = toTime.DayOfYear > fromTime.DayOfYear ? true : false,
                FromTime = fromTime.ToString("HH:mm"),
                ToTime = toTime.ToString("HH:mm")
            };
            return entity;
        }

        /// <summary>
        /// Fills the data for getting keep in view request.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns>The data object of RequestEntity.</returns>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules", "SP2101:MethodMustNotContainMoreLinesThan", Justification = "Reviewed. Suppression is OK here.")]
        private RequestEntity FillDataForGettingKeepInViewRequest(IDataReader reader)
        {
            var entity = new RequestEntity
            {
                Id = reader.ToLong("RequestId"),
                Type = new RequestType { Id = reader.ToShort("RequestType") },
                RequestedDate = reader.ToDateTime("OnDate"),
                IsHalfDay = reader.ToBool("IsHalfDay"),
                FromTime = reader.ToDateTime("FromTime").ToString("HH:mm"),
                ToTime = reader.ToDateTime("ToTime").ToString("HH:mm"),
                Remark = reader.ToString("Remark"),
                Shift = reader.ToString("Shift"),
                CreatedDate = reader.ToDateTime("CreatedDate"),
                Status = new RequestStatus { Id = reader.ToByte("Status") },
                ApprovedDate = reader.ToDateTime("ApprovedDate"),
                VerifiedDate = reader.ToDateTime("VerifiedDate"),
                HrDate = reader.ToDateTime("HrDate"),
                Requester = new Employee
                {
                    NickName = reader.To<string>("Requester"),
                    FullName = reader.To<string>("FullNameRequester"),
                    Team = new Team
                    {
                        Name = reader.To<string>("Team"),
                        Code = reader.To<string>("TeamCode")
                    }
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
