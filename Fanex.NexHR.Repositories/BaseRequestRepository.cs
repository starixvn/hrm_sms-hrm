// --------------------------------------------------------------------------
// <copyright file="BaseRequestRepository.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2014.1.10">Created</change>
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

    /// <summary>
    /// Request Base Repository.
    /// </summary>
    public class BaseRequestRepository
    {
        public virtual long Insert(object model, out int errorCode, out string message)
        {
            errorCode = 0;
            message = string.Empty;
            return 0;
        }

        public virtual bool Update(object model, out int errorCode, out string message)
        {
            errorCode = 0;
            message = string.Empty;
            return true;
        }

        /// <summary>
        /// Method Get all request.
        /// </summary>
        /// <param name="requestType">Request Type.</param>
        /// <param name="role">Employee's role id.</param>
        /// <param name="fromDate">From date.</param>
        /// <param name="toDate">Get to date.</param>
        /// <param name="groupIds">Group's id.</param>
        /// <param name="status">Request status.</param>
        /// <param name="isDailyRequest">Is Daily request.</param>
        /// <param name="employeeId">Employee id.</param>
        /// <param name="logInId">Log in id.</param>
        /// <param name="isLocal">Expat or Local.</param>
        /// <returns>List of RequestEntity.</returns>
        public virtual IList<RequestEntity> GetRequests(
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
                            var entity = FillDataForAllRequest(reader);
                            entityList.Add(entity);
                        }
                    }
                }
            }

            return entityList;
        }

        /// <summary>
        /// Function to get request by requestId.
        /// </summary>
        /// <param name="requestId">Request Id.</param>
        /// <param name="requestTypeId">RequestType Id.</param>
        /// <returns>Request Entity.</returns>
        public virtual RequestEntity GetRequestById(long requestId, int requestTypeId)
        {
            IList<RequestEntity> entityList = null;
            using (ObjectDb db = new ObjectDb("WorkingTimeRequest_GetAllByRequestID"))
            {
                using (IDataReader reader = db.ExecuteReader(new { RequestId = requestId }))
                {
                    if (reader != null)
                    {
                        entityList = FillDataByRequestType(reader, requestTypeId);
                    }
                }
            }

            return entityList.FirstOrDefault();
        }

        public virtual bool Delete(long requestId)
        {
            using (ObjectDb db = new ObjectDb("WorkingTimeRequest_Delete"))
            {
                db.AddInParameter("@RequestId", DbType.Int64, requestId);
                return db.ExecuteNonQuery() != 0;
            }
        }

        /// <summary>
        /// Function to get History request.
        /// </summary>
        /// <param name="requestId">Request Id.</param>
        /// <param name="requestTypeId">Request Type Id.</param>
        /// <returns>List Request Entity.</returns>
        public virtual IList<RequestEntity> GetHistoryRequest(long requestId, int requestTypeId)
        {
            IList<RequestEntity> entityList = null;
            using (var db = new ObjectDb("WorkingTimeRequest_GetRequestHistory"))
            {
                using (IDataReader reader = db.ExecuteReader(new { requestid = requestId, type = requestTypeId }))
                {
                    if (reader != null)
                    {
                        entityList = FillDataByRequestType(reader, requestTypeId);
                    }
                }
            }

            return entityList;
        }

        /// <summary>
        /// Function to fill data follow request type.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="requestTypeId">RequestType Id.</param>
        /// <returns>List Request Entity.</returns>
        private IList<RequestEntity> FillDataByRequestType(IDataReader reader, int requestTypeId)
        {
            IList<RequestEntity> entityList = null;
            switch (requestTypeId)
            {
                case (int)RequestFilter.CheckInOut:
                    entityList = GetCheckInOutRequest(reader);
                    break;
                case (int)RequestFilter.LateInEarlyOut:
                    entityList = GetLateInEarlyOutRequest(reader);
                    break;
                case (int)RequestFilter.ChangeShift:
                    entityList = GetChangeShiftRequest(reader);
                    break;
                case (int)RequestFilter.Leave:
                    entityList = GetLeaveRequest(reader);
                    break;
                case (int)RequestFilter.KeepInView:
                    entityList = GetKeepInViewRequest(reader);
                    break;
            }

            return entityList;
        }

        /// <summary>
        /// Fills the data for getting leave request.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns>The object data of RequestEntity.</returns>
        private IList<RequestEntity> GetLeaveRequest(IDataReader reader)
        {
            IList<RequestEntity> entityList = new List<RequestEntity>();
            while (reader.Read())
            {
                var entity = FillDataForLeaveRequest(reader);
                entityList.Add(entity);
            }

            return entityList;
        }

        /// <summary>
        /// Fills the data for getting leave request.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns>The object data of RequestEntity.</returns>
        private IList<RequestEntity> GetKeepInViewRequest(IDataReader reader)
        {
            IList<RequestEntity> entityList = new List<RequestEntity>();
            while (reader.Read())
            {
                var entity = FillDataForKeepInViewRequest(reader);
                entityList.Add(entity);
            }

            return entityList;
        }

        /// <summary>
        /// Fills the data for getting change shift request.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns>The list change shift object.</returns>
        private IList<RequestEntity> GetChangeShiftRequest(IDataReader reader)
        {
            IList<RequestEntity> entityList = new List<RequestEntity>();
            while (reader.Read())
            {
                var entity = FillDataForChangeShiftRequest(reader);
                entityList.Add(entity);
            }

            return entityList;
        }

        private IList<RequestEntity> GetCheckInOutRequest(IDataReader reader)
        {
            IList<RequestEntity> entityList = new List<RequestEntity>();
            while (reader.Read())
            {
                var entity = FillDataForCheckInOutRequest(reader);
                entityList.Add(entity);
            }

            return entityList;
        }

        private IList<RequestEntity> GetLateInEarlyOutRequest(IDataReader reader)
        {
            IList<RequestEntity> entityList = new List<RequestEntity>();
            while (reader.Read())
            {
                var entity = FillDataForLateInEarlyOutRequest(reader);
                entityList.Add(entity);
            }

            return entityList;
        }

        /// <summary>
        /// Fills the data for getting leave request.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns>The data object of RequestEntity.</returns>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules", "SP2101:MethodMustNotContainMoreLinesThan", Justification = "Reviewed. Suppression is OK here.")]
        private RequestEntity FillDataForLeaveRequest(IDataReader reader)
        {
            var entity = new RequestEntity
            {
                Id = reader.ToInt("RequestId"),
                Type = new RequestType { Id = reader.ToShort("RequestType") },
                FromDate = reader.ToDateTime("FromDate"),
                ToDate = reader.ToDateTime("ToDate"),
                TotalDays = reader.ToDecimal("TotalDays"),
                IsHalfDay = reader.ToDouble("TotalDays") == 0.5 && reader.ToInt("LeaveType") != 0,
                FromTime = ColumnExists(reader, "FromTime") ? reader.ToDateTime("FromTime") : reader.ToDateTime("FromDate"),
                ToTime = ColumnExists(reader, "ToTime") ? reader.ToDateTime("ToTime") : reader.ToDateTime("ToDate"),
                LeaveType = new LeaveType
                {
                    Id = reader.ToInt("LeaveType"),
                    Name = reader.ToString("LeaveTypeName")
                },
                Remark = reader.ToString("Remark"),
                TotalDaysRequestInMonth = ColumnExists(reader, "TotalDaysRequestInMonth") ? reader.ToDecimal("TotalDaysRequestInMonth") : 0,
                CreatedDate = reader.ToDateTime("CreatedDate"),
                Status = new RequestStatus { Id = reader.ToByte("Status") },
                RefNO = reader.ToString("RefNo"),
                ApprovedDate = reader.ToDateTime("ApprovedDate"),
                VerifiedDate = reader.ToDateTime("VerifiedDate"),
                ProcessedDate = reader.ToDateTime("HrDate"),
                Requester = new Employee
                {
                    Id = this.ColumnExists(reader, "RequesterId") ? reader.To<int>("RequesterId") : 0,
                    NickName = reader.To<string>("Requester"),
                    FullName = reader.To<string>("FullNameRequester"),
                    Team = new Team { Name = reader.To<string>("Team"), Code = reader.To<string>("TeamCode") }
                },
                ModifiedBy = new Employee
                {
                    Id = reader.To<int>("ModifiedByID"),
                    NickName = reader.To<string>("ModifiedBy"),
                    FullName = reader.To<string>("FullNameModified")
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
                ProcessedBy = new Employee
                {
                    Id = reader.To<int>("HrId"),
                    NickName = reader.To<string>("HrBy"),
                    FullName = reader.To<string>("FullNameHrBy"),
                }
            };

            return entity;
        }

        /// <summary>
        /// Fills the data for getting change shift request.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns>The data object of RequestEntity.</returns>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules", "SP2101:MethodMustNotContainMoreLinesThan", Justification = "Reviewed. Suppression is OK here.")]
        private RequestEntity FillDataForLateInEarlyOutRequest(IDataReader reader)
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
                FromDate = reader.ToDateTime("FromDate"),
                ToDate = reader.ToDateTime("ToDate"),
                TotalDaysRequestInMonth = this.ColumnExists(reader, "TotalDaysRequest") ? reader.ToDecimal("TotalDaysRequest") : 0,
                ApprovedDate = reader.ToDateTime("ApprovedDate"),
                VerifiedDate = reader.ToDateTime("VerifiedDate"),
                ProcessedDate = reader.ToDateTime("HrDate"),

                Requester = new Employee
                {
                    Id = reader.To<int>("RequesterID"),
                    NickName = reader.To<string>("Requester"),
                    FullName = reader.To<string>("FullNameRequester"),
                    Team = new Team { Name = reader.To<string>("Team"), Code = reader.To<string>("TeamCode") }
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
                ProcessedBy = new Employee
                {
                    Id = reader.To<int>("HrId"),
                    NickName = reader.To<string>("HrBy"),
                    FullName = reader.To<string>("FullNameHrBy"),
                },

                ModifiedBy = new Employee
                {
                    Id = reader.To<int>("ModifiedByID"),
                    NickName = reader.To<string>("ModifiedBy"),
                    FullName = reader.To<string>("FullNameModified"),
                }
            };

            return entity;
        }

        /// <summary>
        /// Fills the data for getting check in/out request.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns>The data object of RequestEntity.</returns>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules", "SP2101:MethodMustNotContainMoreLinesThan", Justification = "Reviewed. Suppression is OK here.")]
        private RequestEntity FillDataForCheckInOutRequest(IDataReader reader)
        {
            var entity = new RequestEntity
            {
                Id = reader.ToInt("RequestId"),
                Type = new RequestType { Id = reader.ToShort("RequestType") },
                FromShift = new Shift { Name = reader.ToString("Shift") },
                FromDate = reader.ToDateTime("FromDate"),
                RequestedDate = reader.ToDateTime("RequestTime"),
                CreatedDate = reader.ToDateTime("CreatedDate"),
                Status = new RequestStatus { Id = reader.ToByte("Status") },
                EmpId = reader.ToInt("RequesterID"),
                RefNO = reader.ToString("RefNo"),
                Remark = reader.ToString("Remark"),
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
                    FullName = reader.To<string>("FullNameModified")
                },
                VerifiedBy = new Employee
                {
                    Id = reader.To<int>("VerifierID"),
                    NickName = reader.To<string>("VerifierBy"),
                    FullName = reader.To<string>("FullNameVerifierBy")
                },
                ApprovedBy = new Employee
                {
                    Id = reader.To<int>("ApproverID"),
                    NickName = reader.To<string>("ApproverBy"),
                    FullName = reader.To<string>("FullNameApproverBy")
                },
                ProcessedBy = new Employee
                {
                    Id = reader.To<int>("HrId"),
                    NickName = reader.To<string>("HrBy"),
                    FullName = reader.To<string>("FullNameHrBy")
                },
                TotalDaysRequestInMonth = this.ColumnExists(reader, "TotalDaysRequest") ? reader.ToDecimal("TotalDaysRequest") : 0,
                ApprovedDate = reader.ToDateTime("ApprovedDate"),
                VerifiedDate = reader.ToDateTime("VerifiedDate"),
                ProcessedDate = reader.ToDateTime("HrDate")
            };

            return entity;
        }

        /// <summary>
        /// Fills the data for getting keep in view request.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns>The data object of RequestEntity.</returns>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules", "SP2101:MethodMustNotContainMoreLinesThan", Justification = "Reviewed. Suppression is OK here.")]
        private RequestEntity FillDataForKeepInViewRequest(IDataReader reader)
        {
            var fromTime = reader.ToDateTime("FromTime");
            var toTime = reader.ToDateTime("ToTime");

            var entity = new RequestEntity
            {
                Id = reader.ToLong("RequestId"),
                Type = new RequestType { Id = reader.ToShort("RequestType") },
                RequestedDate = reader.ToDateTime("OnDate"),
                Remark = reader.ToString("Remark"),
                Shift = new Shift 
                { 
                    Id = reader.ToInt("ShiftId"), 
                    ShiftID = reader.ToInt("ShiftId"), 
                    Name = reader.ToString("FromShift") 
                },
                CreatedDate = reader.ToDateTime("CreatedDate"),
                Status = new RequestStatus { Id = reader.ToByte("Status") },
                EmpId = reader.ToInt("RequesterID"),
                RefNO = reader.ToString("RefNo"),
                IsHalfDay = reader.ToBool("IsHalfDay"),
                Requester = new Employee
                {
                    Id = reader.ToInt("RequesterID"),
                    NickName = reader.ToString("Requester"),
                    FullName = reader.ToString("FullNameRequester"),
                    Team = new Team 
                    {
                        Name = reader.ToString("Team"),
                        Code = reader.ToString("TeamCode") 
                    }
                },
                VerifiedBy = new Employee
                {
                    NickName = reader.ToString("VerifierBy"),
                    FullName = reader.ToString("FullNameVerifierBy"),
                    Id = reader.ToInt("VerifierID")
                },
                ApprovedBy = new Employee
                {
                    NickName = reader.ToString("ApproverBy"),
                    FullName = reader.ToString("FullNameApproverBy"),
                    Id = reader.To<int>("ApproverID")
                },
                ProcessedBy = new Employee
                {
                    Id = reader.ToInt("HrId"),
                    NickName = reader.ToString("HrBy"),
                    FullName = reader.ToString("FullNameHrBy"),
                },
                CrossDay = toTime.DayOfYear > fromTime.DayOfYear,
                FromTime = fromTime,
                ToTime = toTime,

                ApprovedDate = reader.ToDateTime("ApprovedDate"),
                VerifiedDate = reader.ToDateTime("VerifiedDate"),
                ProcessedDate = reader.ToDateTime("HrDate"),
                
                ModifiedBy = new Employee
                {
                    Id = reader.ToInt("ModifiedByID"),
                    NickName = reader.ToString("ModifiedBy"),
                    FullName = reader.ToString("FullNameModified"),
                }
            };

            return entity;
        }

        /// <summary>
        /// Fills the data for getting change shift request.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns>The data object of RequestEntity.</returns>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules", "SP2101:MethodMustNotContainMoreLinesThan", Justification = "Reviewed. Suppression is OK here.")]
        private RequestEntity FillDataForChangeShiftRequest(IDataReader reader)
        {
            var entity = new RequestEntity
            {
                Id = reader.ToInt("RequestId"),
                Type = new RequestType { Id = reader.ToShort("RequestType") },
                FromShift = new Shift { Name = reader.To<string>("FromShift") },
                ToShift = new Shift { Name = reader.To<string>("ToShift") },
                Remark = reader.ToString("Remark"),
                CreatedDate = reader.ToDateTime("CreatedDate"),
                Status = new RequestStatus { Id = reader.ToByte("Status") },
                RequestedDate = reader.ToDateTime("RequestDate"),
                FromDate = reader.ToDateTime("RequestDate"),
                ToDate = reader.ToDateTime("ToDate"),
                ApprovedDate = reader.ToDateTime("ApprovedDate"),
                VerifiedDate = reader.ToDateTime("VerifiedDate"),
                ProcessedDate = reader.ToDateTime("HrDate"),
                Requester = new Employee
                {
                    Id = this.ColumnExists(reader, "RequesterId") ? reader.To<int>("RequesterId") : 0,
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
                ProcessedBy = new Employee
                {
                    Id = reader.To<int>("HrId"),
                    NickName = reader.To<string>("HrBy"),
                    FullName = reader.To<string>("FullNameHrBy"),
                }
            };

            return entity;
        }

        /// <summary>
        /// Fills the data for getting change shift request.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns>The data object of RequestEntity.</returns>
        private RequestEntity FillDataForAllRequest(IDataReader reader)
        {
            var entity = new RequestEntity
            {
                Id = reader.ToInt("RequestId"),
                Type = new RequestType { Id = reader.ToShort("RequestType") },
                FromShift = new Shift { Name = reader.To<string>("FromShift") },
                ToShift = new Shift { Name = reader.To<string>("ToShift") },
                Remark = reader.ToString("Remark"),
                CreatedDate = reader.ToDateTime("CreatedDate"),
                Status = new RequestStatus { Id = reader.ToByte("Status") },
                RequestedDate = reader.ToDateTime("RequestDate"),
                FromDate = reader.ToDateTime("RequestDate"),
                ToDate = reader.ToDateTime("ToDate"),
                ApprovedDate = reader.ToDateTime("ApprovedDate"),
                VerifiedDate = reader.ToDateTime("VerifiedDate"),
                ProcessedDate = reader.ToDateTime("HrDate"),
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
                ProcessedBy = new Employee
                {
                    Id = reader.To<int>("HrId"),
                    NickName = reader.To<string>("HrBy"),
                    FullName = reader.To<string>("FullNameHrBy"),
                }
            };

            return entity;
        }

        /// <summary>
        /// Function to check column exists.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="columnName">Column Name.</param>
        /// <returns>Result of check.</returns>
        private bool ColumnExists(IDataReader reader, string columnName)
        {
            for (int i = 0; i < reader.FieldCount; i++)
            {
                if (reader.GetName(i).ToUpper() == columnName.ToUpper())
                {
                    return true;
                }
            }

            return false;
        }
    }
}
