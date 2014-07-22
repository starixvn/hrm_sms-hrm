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
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Fanex.Common.Extensions;
    using Fanex.Data;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.Repositories.Interfaces;

    /// <summary>
    /// Request Repository.
    /// </summary>
    public class RequestRepository : IRequestRepository
    {
        public bool DeleteWorkingTimeRequest(long id)
        {
            using (ObjectDb db = new ObjectDb("WorkingTimeRequest_Delete"))
            {
                db.AddInParameter("@RequestId", DbType.Int64, id);
                return db.ExecuteNonQuery() != 0;
            }
        }

        /// <summary>
        /// Method process request.
        /// </summary>
        /// <param name="command">Commands (approve, reject..).</param>
        /// <param name="requestId">Request id.</param>
        /// <param name="roleId">The role identifier.</param>
        /// <param name="processedBy">Processed by.</param>
        /// <param name="remark">Reason to proceed.</param>
        /// <param name="processedDate">Processed date.</param>
        /// <param name="message">Message comment.</param>
        /// <returns>
        /// Number to check status.
        /// </returns>
        public int ProcessRequest(
            string command,
            long requestId,
            int roleId,
            int processedBy,
            string remark,
            DateTime processedDate,
            out string message)
        {
            string key = this.GetSpKeyForProcessRequest(command);
            using (ObjectDb db = new ObjectDb(key))
            {
                if (command != Commands.Reset.ToString())
                {
                    db.AddInParameter("@Comment", DbType.String, remark);
                }

                db.AddInParameter("@RequestID", DbType.Int64, requestId);
                db.AddInParameter("@ProcessedBy", DbType.Int32, processedBy);
                db.AddInParameter("@ProcessedDate", DbType.DateTime, processedDate);
                db.AddInParameter("@RoleID", DbType.Int32, roleId);
                db.AddInParameter("@bOnbehalf", DbType.Boolean, false);
                db.AddOutParameter("@ErrCode", DbType.Int32, 8);
                db.AddOutParameter("@ErrMsg", DbType.String, 200);

                db.ExecuteNonQuery();
                message = db.GetParameterValue("ErrMsg") as string;
                return (int)db.GetParameterValue("ErrCode");
            }
        }

        /// <summary>
        /// Method process overtime.
        /// </summary>
        /// <param name="command">Commands (approve, reject..).</param>
        /// <param name="requestId">Request id.</param>
        /// <param name="roleId">The role identifier.</param>
        /// <param name="processedBy">Processed by.</param>
        /// <param name="processedDate">Processed date.</param>
        /// <param name="message">Message comment.</param>
        /// <returns>Number to check status.</returns>
        public int ProcessOvertime(
            string command,
            long requestId,
            int roleId,
            int processedBy,
            DateTime processedDate,
            out string message)
        {
            string key = this.GetSpKeyForProcessSingleOvertime(command);

            using (ObjectDb db = new ObjectDb(key))
            {
                db.AddInParameter("@RequestID", DbType.Int64, requestId);
                db.AddInParameter("@ProcessedBy", DbType.Int32, processedBy);
                db.AddInParameter("@ProcessedDate", DbType.DateTime, processedDate);
                db.AddInParameter("@RoleID", DbType.Int32, roleId);
                db.AddInParameter("@bOnbehalf", DbType.Boolean, false);
                db.AddOutParameter("@ErrCode", DbType.Int32, 8);
                db.AddOutParameter("@ErrMsg", DbType.String, 200);

                db.ExecuteNonQuery();
                message = db.GetParameterValue("ErrMsg") as string;
                return (int)db.GetParameterValue("ErrCode");
            }
        }

        /// <summary>
        /// Method process overtime.
        /// </summary>
        /// <param name="command">Commands (approve, reject..).</param>
        /// <param name="requestIds">Request ids.</param>
        /// <param name="processedBy">Processed by.</param>
        /// <param name="processedDate">Processed date.</param>
        /// <param name="message">Message comment.</param>
        /// <returns>Number to check status.</returns>
        public int ProcessOvertime(
            string command,
            string requestIds,
            int processedBy,
            DateTime processedDate,
            out string message)
        {
            string key = this.GetSpKeyForProcessMultipleOvertime(command);
            int roleId = this.GetRoleIdForProcessOvertime(command);

            using (ObjectDb db = new ObjectDb(key))
            {
                db.AddInParameter("@ListRequestId", DbType.String, requestIds);
                db.AddInParameter("@ProcessedBy", DbType.Int32, processedBy);
                db.AddInParameter("@ProcessedDate", DbType.DateTime, processedDate);
                db.AddInParameter("@RoleID", DbType.Int32, roleId);
                db.AddInParameter("@bOnbehalf", DbType.Boolean, false);
                db.AddOutParameter("@ErrCode", DbType.Int32, 8);
                db.AddOutParameter("@ErrMsg", DbType.String, 200);

                db.ExecuteNonQuery();
                message = db.GetParameterValue("ErrMsg") as string;
                return (int)db.GetParameterValue("ErrCode");
            }
        }

        public IList<WorkingTimeRequest> GetWorkingTimeRequests(int employeeId, int? requestType = null, byte? status = null)
        {
            using (var db = new ObjectDb("WorkingTimeRequest_GetByEmployee"))
            {
                return db.Query<WorkingTimeRequest>(new { id = employeeId, requestType, status }).ToList();
            }
        }

        public WorkingTimeRequest GetWorkingTimeRequest(long id)
        {
            using (ObjectDb db = new ObjectDb("WorkingTimeRequest_GetAllByRequestID"))
            {
                return db.Query<WorkingTimeRequest>(new { RequestID = id }).FirstOrDefault();
            }
        }

        /// <summary>
        /// Method Get all request.
        /// </summary>
        /// <param name="requestType">Request Type.</param>
        /// <param name="role">Employee's role id.</param>
        /// <param name="fromDate">From date.</param>
        /// <param name="toDate">The to date.</param>
        /// <param name="groupIds">Group's id.</param>
        /// <param name="status">Request status.</param>
        /// <param name="isDailyRequest">Is Daily request.</param>
        /// <param name="employeeId">Employee id.</param>
        /// <param name="logInId">Log in id.</param>
        /// <param name="isLocal">Expat or Local.</param>
        /// <returns>List of RequestEntity.</returns>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules", "SP2101:MethodMustNotContainMoreLinesThan", Justification = "Reviewed. Suppression is OK here.")]
        public IList<RequestEntity> GetRequests(
            int requestType,
            string role,
            DateTime fromDate,
            DateTime toDate,
            string groupIds,
            string status,
            bool isDailyRequest = false,
            int? employeeId = null,
            int? logInId = null,
            byte? isLocal = null)
        {
            IList<RequestEntity> entityList = new List<RequestEntity>();
            var keyName = isDailyRequest
                        ? "WorkingTimeRequest_GetRequestByRequestDate_AllType"
                        : "WorkingTimeRequest_GetRequestByCreateDate_AllType";

            using (var db = new ObjectDb(keyName))
            {
                using (IDataReader reader = db.ExecuteReader(
                    new { requestType, empid = employeeId, groupIds, fromDate, toDate, status, excludedEmpID = logInId, role = role, supervisorID = logInId, isLocal }))
                {
                    if (reader != null)
                    {
                        while (reader.Read())
                        {
                            var entity = new RequestEntity
                            {
                                Id = reader.ToInt("RequestId"),
                                CreatedDate = reader.ToDateTime("CreatedDate"),
                                Status = new RequestStatus { Id = reader.ToByte("Status") },
                                Type = new RequestType { Id = reader.ToShort("RequestType") },
                                TotalDaysRequestInMonth = reader.ToDecimal("TotalDaysRequestInMonth"),
                                RequestedDate = reader.ToDateTime("RequestDate"),
                                Requester = new Employee
                                {
                                    Id = reader.To<int>("RequesterID"),
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
                                    Id = reader.To<int>("ApproverId"),
                                    NickName = reader.To<string>("ApproverBy"),
                                    FullName = reader.To<string>("FullNameApproverBy"),
                                },
                                ProcessedBy = new Employee
                                {
                                    Id = reader.To<int>("HrId"),
                                    NickName = reader.To<string>("HrBy"),
                                    FullName = reader.To<string>("FullNameHrBy"),
                                },
                                ModifiedDate = reader.ToDateTime("ModifiedDate"),
                                FromShift = new Shift { Name = reader.To<string>("FromShift") },
                                ToShift = new Shift { Name = reader.To<string>("ToShift") },
                                TotalMins = reader.ToInt("Time"),
                                FromDate = reader.ToDateTime("FromDate"),
                                ToDate = reader.ToDateTime("ToDate"),
                                FromTime = reader.ToDateTime("FromTime"),
                                ToTime = reader.ToDateTime("ToTime"),
                                TotalDays = reader.ToDecimal("TotalDays"),
                                LeaveType = new LeaveType
                                {
                                    Id = reader.ToInt("LeaveTypeId"),
                                    Name = reader.ToString("LeaveType")
                                },
                                Remark = reader.ToString("Remark"),
                                VerifiedStatus = reader.ToInt("VerifiedStatus"),
                                ApprovedStatus = reader.ToInt("ApprovedStatus"),
                                ProcessedStatus = reader.ToInt("HRStatus"),
                                ApprovedDate = reader.ToDateTime("ApprovedDate"),
                                VerifiedDate = reader.ToDateTime("VerifiedDate"),
                                ProcessedDate = reader.ToDateTime("HrDate"),
                                RefNO = reader.ToString("RefNo")
                            };
                            entityList.Add(entity);
                        }
                    }
                }
            }

            return entityList;
        }

        public IList<SupervisorNameModel> GetSupervisorByQuery(int empId, int role, int? requestType, string query, bool includeLoginUser)
        {
            using (ObjectDb db = new ObjectDb("EmployeeSupervisor_GetByQuery"))
            {
                var supervisorList = db.Query<SupervisorNameModel>(
                    new { empId, role, requestType, query, includeLoginUser }).ToList();
                return supervisorList;
            }
        }

        /// <summary>
        /// Function to get list supervisor related with request.
        /// </summary>
        /// <param name="requestId">Request id.</param>
        /// <param name="action">Action process request.</param>
        /// <param name="employee">Output Employee.</param>
        /// <param name="verifier">Output Verifier.</param>
        /// <param name="approver">Output Approver.</param>
        /// <param name="listHr">Output SupervisorNameModel list.</param>
        /// <param name="emailSettings">Email settings.</param>
        public void GetSupervisorAndHrByRequestId(
            long requestId,
            int action,
            ref SupervisorNameModel employee,
            ref SupervisorNameModel verifier,
            ref SupervisorNameModel approver,
            ref IList<SupervisorNameModel> listHr,
            ref EmailSettingsModel emailSettings)
        {
            using (IObjectDb db = new ObjectDb("EmployeeSupervisor_GetListSupervisorRequestID"))
            {
                using (var multi = db.QueryMultiple(new { requestId = requestId, action = action }))
                {
                    employee = multi.Read<SupervisorNameModel>().FirstOrDefault();
                    verifier = multi.Read<SupervisorNameModel>().FirstOrDefault();
                    approver = multi.Read<SupervisorNameModel>().FirstOrDefault();
                    listHr = multi.Read<SupervisorNameModel>().ToList();
                    emailSettings = multi.Read<EmailSettingsModel>().FirstOrDefault();
                }
            }
        }

        /// <summary>
        /// Gets the role identifier for process request.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns>The role id of processor.</returns>
        private int GetRoleIdForProcessOvertime(string command)
        {
            int roleId = 0;

            if (command == Commands.Accept.ToString()
                || command == Commands.NotAccept.ToString())
            {
                roleId = (int)RolesEnum.Verifier;
            }
            else if (command == Commands.Approve.ToString()
                || command == Commands.Reject.ToString())
            {
                roleId = (int)RolesEnum.Approver;
            }
            else if (command == Commands.Process.ToString()
                || command == Commands.Void.ToString())
            {
                roleId = (int)RolesEnum.HR;
            }

            return roleId;
        }

        /// <summary>
        /// Gets the key for process request.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns>The key name for processing request.</returns>
        private string GetSpKeyForProcessRequest(string command)
        {
            string key = string.Empty;

            if (command == Commands.Accept.ToString()
                || command == Commands.Approve.ToString()
                || command == Commands.Process.ToString())
            {
                key = "WorkingTimeRequest_ApproveRequest";
            }
            else if (command == Commands.NotAccept.ToString()
                || command == Commands.Reject.ToString()
                || command == Commands.Void.ToString())
            {
                key = "WorkingTimeRequest_RejectRequest";
            }
            else if (command == Commands.Reset.ToString())
            {
                key = "WorkingTimeRequest_ResetRequest";
            }

            return key;
        }

        /// <summary>
        /// Gets the key for process request.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns>The key name for processing request.</returns>
        private string GetSpKeyForProcessSingleOvertime(string command)
        {
            string key = string.Empty;

            if (command == Commands.Accept.ToString()
                || command == Commands.Approve.ToString()
                || command == Commands.Process.ToString())
            {
                key = "OverTime_ApproveSingleRequest";
            }
            else if (command == Commands.NotAccept.ToString()
                || command == Commands.Reject.ToString()
                || command == Commands.Void.ToString())
            {
                key = "OverTime_RejectSingleRequest";
            }
            else
            {
                key = "Overtime_ResetSingleRequest";
            }

            return key;
        }

        /// <summary>
        /// Gets the key for process request.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns>The key name for processing request.</returns>
        private string GetSpKeyForProcessMultipleOvertime(string command)
        {
            string key = string.Empty;

            if (command == Commands.Accept.ToString()
                || command == Commands.Approve.ToString()
                || command == Commands.Process.ToString())
            {
                key = "OverTime_ApproveMultiRequest";
            }
            else if (command == Commands.NotAccept.ToString()
                || command == Commands.Reject.ToString()
                || command == Commands.Void.ToString())
            {
                key = "OverTime_RejectMultiRequest";
            }
            else
            {
                key = "Overtime_ResetMultiRequest";
            }

            return key;
        }
    }
}
