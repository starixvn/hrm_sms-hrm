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
    using Fanex.Data.Repository;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Criteria.Request;
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
        /// <param name="command">Command (approve, reject..).</param>
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
                if (command != Command.Reset.ToString())
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
        /// <param name="command">Command (approve, reject..).</param>
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
        /// <param name="command">Command (approve, reject..).</param>
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
                db.AddInParameter("@Onbehalf", DbType.Boolean, false);
                db.AddOutParameter("@ErrCode", DbType.Int32, 8);
                db.AddOutParameter("@ErrMsg", DbType.String, 200);

                db.ExecuteNonQuery();
                message = db.GetParameterValue("ErrMsg") as string;
                return (int)db.GetParameterValue("ErrCode");
            }
        }

        public WorkingTimeRequest GetWorkingTimeRequest(long requestId)
        {
            using (ObjectDb db = new ObjectDb("WorkingTimeRequest_GetAllByRequestID"))
            {
                return db.Query<WorkingTimeRequest>(new { RequestID = requestId }).FirstOrDefault();
            }
        }

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
        /// <param name="requestId">Request Id.</param>
        /// <param name="requestTypeId">Request Type Id.</param>
        /// <returns>List Request Entity.</returns>
        public IList<RequestEntity> GetHistoryRequest(long requestId, int requestTypeId)
        {
            IList<RequestEntity> entityList = null;
            using (var db = new ObjectDb("WorkingTimeRequest_GetRequestHistory"))
            {
                using (IDataReader reader = db.ExecuteReader(new { requestid = requestId, type = requestTypeId }))
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
                        while (reader.Read())
                        {
                            var entity = new RequestEntity
                            {
                                Id = reader.ToInt("RequestId"),
                                CreatedDate = reader.ToDateTime("CreatedDate"),
                                Status = new RequestStatus { Id = reader.ToByte("Status") },
                                RequestType = new RequestTypeModel { Id = reader.ToShort("RequestType") },
                                TotalDaysRequestInMonth = reader.ToDecimal("TotalDaysRequestInMonth"),
                                RequestedDate = reader.ToDateTime("RequestDate"),
                                IsWarning = reader.ToBool("IsWarning"),
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
                                IsHalfDay = reader.ToShort("RequestType") == 7 && reader.ToDouble("TotalDays") == 0.5,
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

        /// <summary>
        /// Gets the role identifier for process request.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns>The role id of processor.</returns>
        private int GetRoleIdForProcessOvertime(string command)
        {
            int roleId = 0;

            if (command == Command.Accept.ToString()
                || command == Command.NotAccept.ToString())
            {
                roleId = (int)RoleSystem.Verifier;
            }
            else if (command == Command.Approve.ToString()
                || command == Command.Reject.ToString())
            {
                roleId = (int)RoleSystem.Approver;
            }
            else if (command == Command.Process.ToString()
                || command == Command.Void.ToString())
            {
                roleId = (int)RoleSystem.HR;
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

            if (command == Command.Accept.ToString()
                || command == Command.Approve.ToString()
                || command == Command.Process.ToString())
            {
                key = "WorkingTimeRequest_ApproveRequest";
            }
            else if (command == Command.NotAccept.ToString()
                || command == Command.Reject.ToString()
                || command == Command.Void.ToString())
            {
                key = "WorkingTimeRequest_RejectRequest";
            }
            else if (command == Command.Reset.ToString())
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

            if (command == Command.Accept.ToString()
                || command == Command.Approve.ToString()
                || command == Command.Process.ToString())
            {
                key = "OverTime_ApproveSingleRequest";
            }
            else if (command == Command.NotAccept.ToString()
                || command == Command.Reject.ToString()
                || command == Command.Void.ToString())
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

            if (command == Command.Accept.ToString()
                || command == Command.Approve.ToString()
                || command == Command.Process.ToString())
            {
                key = "OverTime_ApproveMultiRequest";
            }
            else if (command == Command.NotAccept.ToString()
                || command == Command.Reject.ToString()
                || command == Command.Void.ToString())
            {
                key = "OverTime_RejectMultiRequest";
            }
            else
            {
                key = "Overtime_ResetMultiRequest";
            }

            return key;
        }

        private IList<RequestEntity> FillData(IDataReader reader)
        {
            IList<RequestEntity> entityList = new List<RequestEntity>();
            while (reader.Read())
            {
                var entity = FillDataEntity(reader);
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
                FromDate = reader.ToDateTime("FromDate"),
                ToDate = reader.ToDateTime("ToDate"),
                TotalDays = reader.ToDecimal("TotalDays"),
                IsHalfDay = reader.ToDouble("TotalDays") == 0.5 && reader.ToInt("LeaveType") != 0,
                ////keep in view reader.ToBool("IsHalfDay")
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
                Status = new RequestStatus { Id = reader.ToByte("Status") },
                RefNO = reader.ToString("RefNo"),
                ApprovedDate = reader.ToDateTime("ApprovedDate"),
                VerifiedDate = reader.ToDateTime("VerifiedDate"),
                ProcessedDate = reader.ToDateTime("HrDate"),
                Requester = new Employee
                {
                    Id = reader.ToInt("RequesterId"),
                    NickName = reader.ToString("Requester"),
                    FullName = reader.ToString("FullNameRequester"),
                    Team = new Team { Name = reader.ToString("Team"), Code = reader.ToString("TeamCode") }
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
                    ShiftID = reader.ToInt("ShiftId"),
                    Name = reader.ToString("ShiftName")
                },
                RequestedDate = reader.ToDateTime("RequestedDate"),
                TotalMins = reader.ToInt("Time"),
                //// Keep in view
                CrossDay = reader.ToDateTime("FromTime").DayOfYear > reader.ToDateTime("ToTime").DayOfYear,
                //// Change shift
                ToShift = new Shift
                {
                    Id = reader.ToInt("ToShiftId"),
                    ShiftID = reader.ToInt("ToShiftId"),
                    Name = reader.ToString("ToShiftName")
                },
            };

            return entity;
        }
    }
}
