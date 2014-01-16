// --------------------------------------------------------------------------
// <copyright file="ChangeShiftRepository.cs" company="Nexcel Solutions Vietnam">
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
    using Fanex.NexHR.ViewModels;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class ChangeShiftRepository : RequestRepository, IChangeShiftRepository
    {
        /// <summary>
        /// Inserts the change shift request.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="errorCode">The error code.</param>
        /// <param name="message">The message.</param>
        /// <returns>Request's id.</returns>
        public long InsertChangeShiftRequest(ChangeShiftViewModel model, out int errorCode, out string message)
        {
            var fromDate = Convert.ToDateTime(model.FromDate);
            var toDate = Convert.ToDateTime(model.ToDate);
            using (var db = new ObjectDb("WorkingTimeRequest_Insert_ChangeShift"))
            {
                db.AddInParameter("@EmpId", DbType.Int32, model.Employee);
                db.AddInParameter("@RequestType", DbType.Int16, model.RequestType);
                db.AddInParameter("@Remark", DbType.String, model.Remark);
                db.AddInParameter("@FromDate", DbType.DateTime, fromDate);
                db.AddInParameter("@ToDate", DbType.DateTime, toDate);
                db.AddInParameter("@CreatedDate", DbType.DateTime, DateTime.Now);
                db.AddInParameter("@CreatedBy", DbType.Int32, model.By);
                db.AddInParameter("@VerifierId", DbType.Int32, model.VerifierId);
                db.AddInParameter("@ApproverId", DbType.Int32, model.ApproverId);
                db.AddInParameter("@HRId", DbType.Int32, null);
                db.AddInParameter("@ListDetail", DbType.Xml, model.Value);

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
        /// Updates the change shift request.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="errorCode">The error code.</param>
        /// <param name="message">The message.</param>
        /// <returns>Error code.</returns>
        public long UpdateChangeShiftRequest(ChangeShiftViewModel model, out int errorCode, out string message)
        {
            var fromDate = Convert.ToDateTime(model.FromDate);
            var toDate = Convert.ToDateTime(model.ToDate);

            using (var db = new ObjectDb("WorkingTimeRequest_Update_ChangeShiftRequest"))
            {
                db.AddInParameter("@RequestId", DbType.Int32, model.Id);
                db.AddInParameter("@EmpId", DbType.Int32, model.Employee);
                db.AddInParameter("@RequestType", DbType.Int16, model.RequestType);
                db.AddInParameter("@Remark", DbType.String, model.Remark);
                db.AddInParameter("@FromDate", DbType.DateTime, fromDate);
                db.AddInParameter("@ToDate", DbType.DateTime, toDate);
                db.AddInParameter("@ModifiedDate", DbType.DateTime, DateTime.Now);
                db.AddInParameter("@ModifiedBy", DbType.Int32, model.By);
                db.AddInParameter("@VerifierId", DbType.Int32, model.VerifierId);
                db.AddInParameter("@ApproverId", DbType.Int32, model.ApproverId);
                db.AddInParameter("@HRId", DbType.Int32, null);

                db.AddInParameter("@ListDetail", DbType.Xml, model.Value);

                db.AddOutParameter("@ErrCode", DbType.Int32, 8);
                db.AddOutParameter("@ErrMsg", DbType.String, 200);

                db.ExecuteScalar();
                message = db.GetParameterValue("ErrMsg") as string;
                errorCode = (int)db.GetParameterValue("ErrCode");

                return errorCode;
            }
        }

        /// <summary>
        /// Method get change shift request.
        /// </summary>
        /// <param name="role">Employee's role id.</param>
        /// <param name="fromDate">From date.</param>
        /// <param name="toDate">Request to date.</param>
        /// <param name="groupIds">Group's id.</param>
        /// <param name="isDailyRequest">Is daily request.</param>
        /// <param name="empid">Employee's id.</param>
        /// <param name="status">Request's status.</param>
        /// <param name="excludedEmpID">Log in id.</param>
        /// <returns>List of RequestEntity.</returns>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules", "SP2101:MethodMustNotContainMoreLinesThan", Justification = "Reviewed. Suppression is OK here.")]
        public IList<RequestEntity> GetChangeShiftRequests(
            string role,
            DateTime fromDate,
            DateTime toDate,
            string groupIds,
            bool isDailyRequest = false,
            int? empid = null,
            byte? status = null,
            int? excludedEmpID = null)
        {
            IList<RequestEntity> entityList = new List<RequestEntity>();
            var keyName = isDailyRequest
                ? "WorkingTimeRequest_GetRequestByRequestDate_ChangeShiftType"
                : "WorkingTimeRequest_GetRequestByCreateDate_ChangeShiftType";
            using (var db = new ObjectDb(keyName))
            {
                using (IDataReader reader = db.ExecuteReader(new { empid, groupIds, fromDate, toDate, status, excludedEmpID, role, SupervisorID = excludedEmpID }))
                {
                    if (reader != null)
                    {
                        while (reader.Read())
                        {
                            var entity = this.FillDataForGettingChangeShiftRequest(reader);
                            entityList.Add(entity);
                        }
                    }
                }
            }

            return entityList;
        }

        /// <summary>
        /// Gets the shift by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>Request Entity.</returns>
        public RequestEntity GetChangeShiftRequestById(long id)
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
                                FromDate = reader.ToDateTime("FromDate"),
                                ToDate = reader.ToDateTime("ToDate"),
                                Status = new RequestStatus { Id = reader.ToByte("Status") },
                                EmpId = reader.ToInt("RequesterID"),
                                Remark = reader.ToString("Remark"),
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

        public List<ShiftDetails> GetShiftDetailsById(long id)
        {
            using (ObjectDb db = new ObjectDb("WorkingTimeRequest_GetChangeShiftTypeDetailByRequestID"))
            {
                return db.Query<ShiftDetails>(new { RequestID = id }).ToList();
            }
        }

        /// <summary>
        /// Method GetChangeShiftRequestHistory.
        /// </summary>
        /// <param name="requestId">Request id.</param>
        /// <param name="requestTypeId">Request type id.</param>
        /// <returns>List of RequestEntity.</returns>
        public IList<RequestEntity> GetHistoricChangeShiftRequests(long requestId, int requestTypeId)
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
                            var entity = this.FillDataForGettingChangeShiftRequest(reader);
                            entityList.Add(entity);
                        }
                    }
                }
            }

            return entityList;
        }

        /// <summary>
        /// Fills the data for getting change shift request.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns>The data object of RequestEntity.</returns>
        private RequestEntity FillDataForGettingChangeShiftRequest(IDataReader reader)
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
