// --------------------------------------------------------------------------
// <copyright file="LeaveRequestRepository.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.11.11">Created</change>
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
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Request;

    /// <summary>
    /// LeaveRequest Repository.
    /// </summary>
    public class LeaveRequestRepository : BaseRequestRepository
    {
        /// <summary>
        /// Function to insert leave request.
        /// </summary>
        /// <param name="model">LeaveRequest Model.</param>
        /// <param name="errorCode">Error code.</param>
        /// <param name="message">Error message.</param>
        /// <returns>Result of insert.</returns>
        public override long Insert(object model, out int errorCode, out string message)
        {
            var leaveRequest = (LeaveRequestModel)model;

            using (var db = new ObjectDb("WorkingTimeRequest_Insert_LeaveMultiRequest"))
            {
                db.AddInParameter("@EmpId", DbType.Int32, leaveRequest.EmployeeId);
                db.AddInParameter("@RequestType", DbType.Int16, (int)RequestTypes.Leave);
                db.AddInParameter("@LeaveTypeID", DbType.Int32, leaveRequest.LeaveTypeId);
                db.AddInParameter("@Remark", DbType.String, leaveRequest.Remark);
                db.AddInParameter("@FromDate", DbType.DateTime, leaveRequest.FromDate);
                db.AddInParameter("@ToDate", DbType.DateTime, leaveRequest.ToDate);
                db.AddInParameter("@TotalDays", DbType.Decimal, leaveRequest.TotalDays);
                db.AddInParameter("@CreatedDate", DbType.DateTime, leaveRequest.CreatedDate);
                db.AddInParameter("@CreatedBy", DbType.Int32, leaveRequest.CreatedBy);
                db.AddInParameter("@VerifierId", DbType.Int32, leaveRequest.VerifierId);
                db.AddInParameter("@ApproverId", DbType.Int32, leaveRequest.ApproverId);
                db.AddInParameter("@RefNo", DbType.String, leaveRequest.RefNo);
                db.AddInParameter("@ListDetail", DbType.String, leaveRequest.RequestDetailXML);

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
        /// Function to update leave request.
        /// </summary>
        /// <param name="model">LeaveRequest Model.</param>
        /// <param name="errorCode">Error Code.</param>
        /// <param name="message">Error message.</param>
        /// <returns>Result of update.</returns>
        public override bool Update(object model, out int errorCode, out string message)
        {
            var leaveRequest = (LeaveRequestModel)model;

            using (var db = new ObjectDb("WorkingTimeRequest_Update_LeaveMultiRequest"))
            {
                db.AddInParameter("@RequestId", DbType.Int64, leaveRequest.RequestId);
                db.AddInParameter("@EmpId", DbType.Int32, leaveRequest.EmployeeId);
                db.AddInParameter("@RequestType", DbType.Int16, (int)RequestTypes.Leave);
                db.AddInParameter("@LeaveTypeID", DbType.Int32, leaveRequest.LeaveTypeId);
                db.AddInParameter("@Remark", DbType.String, leaveRequest.Remark);
                db.AddInParameter("@FromDate", DbType.DateTime, leaveRequest.FromDate);
                db.AddInParameter("@ToDate", DbType.DateTime, leaveRequest.ToDate);
                db.AddInParameter("@TotalDays", DbType.Decimal, leaveRequest.TotalDays);
                db.AddInParameter("@ModifiedDate", DbType.DateTime, DateTime.Now);
                db.AddInParameter("@ModifiedBy", DbType.Int32, leaveRequest.CreatedBy);
                db.AddInParameter("@VerifierId", DbType.Int32, leaveRequest.VerifierId);
                db.AddInParameter("@ApproverId", DbType.Int32, leaveRequest.ApproverId);
                db.AddInParameter("@RefNo", DbType.String, leaveRequest.RefNo);
                db.AddInParameter("@ListDetail", DbType.String, leaveRequest.RequestDetailXML);

                db.AddOutParameter("@ErrCode", DbType.Int32, 8);
                db.AddOutParameter("@ErrMsg", DbType.String, 200);

                var result = db.ExecuteNonQuery() != 0;

                message = db.GetParameterValue("ErrMsg") as string;
                errorCode = (int)db.GetParameterValue("ErrCode");

                return result;
            }
        }

        /// <summary>
        /// Method get leave request.
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
        /// <returns>List RequestEntity.</returns>
        public override IList<RequestEntity> GetRequests(
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
            IList<RequestEntity> entityList = null;
            var keyName = isDailyRequest
                ? "WorkingTimeRequest_GetRequestByRequestDate_LeaveType"
                : "WorkingTimeRequest_GetRequestByCreateDate_LeaveType";
            using (var db = new ObjectDb(keyName))
            {
                using (IDataReader reader =
                    db.ExecuteReader(new { empid = employeeId, groupIds, fromDate, toDate, status, excludedEmpID = logInId, role = role, supervisorID = logInId, isLocal }))
                {
                    if (reader != null)
                    {
                        entityList = new List<RequestEntity>();
                        while (reader.Read())
                        {
                            var entity = this.FillDataForGettingLeaveRequest(reader);
                            entityList.Add(entity);
                        }
                    }
                }
            }

            return entityList;
        }

        public List<LeaveDetailModel> GetLeaveRequestDetailsById(long requestId)
        {
            using (ObjectDb db = new ObjectDb("WorkingTimeRequest_GetLeaveRequestDetailByRequestID"))
            {
                var leaveRequestDetails = db.Query<LeaveDetailModel>(new { RequestID = requestId }).ToList();
                return leaveRequestDetails;
            }
        }

        /// <summary>
        /// Function to get leave request by id.
        /// </summary>
        /// <param name="requestId">Request Id.</param>
        /// <returns>Request Entity.</returns>
        public override object GetRequestById(long requestId)
        {
            IList<RequestEntity> entityList = null;
            using (ObjectDb db = new ObjectDb(Constants.WorkingTimeRequestGetAllByRequestID))
            {
                using (IDataReader reader = db.ExecuteReader(new { RequestId = requestId }))
                {
                    if (reader != null)
                    {
                        entityList = new List<RequestEntity>();
                        while (reader.Read())
                        {
                            var entity = this.FillDataForGettingLeaveRequestById(reader);
                            entityList.Add(entity);
                        }
                    }
                }
            }

            return entityList.FirstOrDefault();
        }

        public EmployeeAnnualBalance GetAnnualLeaveAndKeepInViewBalance(int employeeId, long? requestId, DateTime effectDay)
        {
            using (ObjectDb db = new ObjectDb("WorkingTimeRequest_AvaibleBalanceLeave"))
            {
                var employeeBalance = db.QueryEntity<EmployeeAnnualBalance>(new { empId = employeeId, requestId = requestId, EffectedDay = effectDay });

                return employeeBalance;
            }
        }

        /// <summary>
        /// Function to reset request when user reedit a leave request.
        /// </summary>
        /// <param name="requestId">Request Id.</param>
        /// <param name="employeeId">User reedit.</param>
        /// <param name="errorCode">Error Code.</param>
        /// <param name="message">Error Message.</param>
        /// <returns>Result of reset request.</returns>
        public bool ResetByUser(long requestId, int employeeId, out int errorCode, out string message)
        {
            using (var db = new ObjectDb("WorkingTimeRequest_ResetRequest_byMember"))
            {
                db.AddInParameter("@RequestId", DbType.Int64, requestId);
                db.AddInParameter("@EmpId", DbType.Int64, employeeId);
                db.AddOutParameter("@ErrCode", DbType.Int32, 8);
                db.AddOutParameter("@ErrMsg", DbType.String, 200);

                var result = db.ExecuteNonQuery() != 0;

                message = db.GetParameterValue("ErrMsg") as string;
                errorCode = (int)db.GetParameterValue("ErrCode");

                return result;
            }
        }

        /// <summary>
        /// Fills the data for getting leave request by identifier.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns>The object data of RequestEntity.</returns>
        private RequestEntity FillDataForGettingLeaveRequestById(IDataReader reader)
        {
            var entity = new RequestEntity
            {
                Id = reader.ToInt("RequestId"),
                Type = new RequestType { Id = reader.ToShort("RequestType") },
                FromDate = reader.ToDateTime("FromDate"),
                ToDate = reader.ToDateTime("ToDate"),
                Status = new RequestStatus { Id = reader.ToByte("Status") },
                EmpId = reader.ToInt("RequesterID"),
                Remark = reader.ToString("Remark"),
                IsHalfDay = reader.ToDouble("TotalDays") == 0.5,
                TotalDays = reader.ToDecimal("TotalDays"),
                LeaveTypeId = reader.ToInt("LeaveType"),
                RefNO = reader.ToString("RefNo"),
                Requester = new Employee
                {
                    Id = reader.To<int>("RequesterId"),
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
            return entity;
        }

        /// <summary>
        /// Fills the data for getting leave request.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns>The object data of RequestEntity.</returns>
        private RequestEntity FillDataForGettingLeaveRequest(IDataReader reader)
        {
            var entity = new RequestEntity
            {
                Id = reader.ToInt("RequestId"),
                Type = new RequestType { Id = reader.ToShort("RequestType") },
                FromDate = reader.ToDateTime("FromDate"),
                ToDate = reader.ToDateTime("ToDate"),
                TotalDays = reader.ToDecimal("TotalDays"),
                LeaveType = reader.ToString("LeaveType"),
                Remark = reader.ToString("Remark"),
                TotalDaysRequestInMonth = reader.ToDecimal("TotalDaysRequestInMonth"),
                CreatedDate = reader.ToDateTime("CreatedDate"),
                Status = new RequestStatus { Id = reader.ToByte("Status") },
                RefNO = reader.ToString("RefNo"),
                ApprovedDate = reader.ToDateTime2("ApprovedDate"),
                VerifiedDate = reader.ToDateTime2("VerifiedDate"),
                HrDate = reader.ToDateTime2("HrDate"),
                VerifiedStatus = reader.ToInt("VerifiedStatus"),
                ApprovedStatus = reader.ToInt("ApprovedStatus"),
                ProcessedStatus = reader.ToInt("HRStatus"),
                RequestedDate = reader.ToDateTime("FromDate"),
                Requester = new Employee
                {
                    NickName = reader.To<string>("Requester"),
                    FullName = reader.To<string>("FullNameRequester"),
                    Team = new Team { Name = reader.To<string>("Team"), Code = reader.To<string>("TeamCode") }
                },
                ModifiedBy = new Employee { Id = reader.To<int>("ModifiedByID"), NickName = reader.To<string>("ModifiedBy"), FullName = reader.To<string>("FullNameModified") },
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
