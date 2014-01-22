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
    using Fanex.Common.Extensions;
    using Fanex.Data;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Request;

    /// <summary>
    /// Request Base Repository.
    /// </summary>
    public abstract class BaseRequestRepository
    {
        public abstract long Insert(object model, out int errorCode, out string message);

        public abstract bool Update(object model, out int errorCode, out string message);

        public abstract IList<RequestEntity> GetRequests(
            string role,
            DateTime fromDate,
            DateTime toDate,
            string groupIds,
            bool isDailyRequest = false,
            int? employeeId = null,
            byte? status = null,
            int? logInId = null,
            byte? isLocal = null);

        public abstract object GetRequestById(long requestId);

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
        public IList<RequestEntity> GetHistoryRequest(long requestId, int requestTypeId)
        {
            IList<RequestEntity> entityList = null;
            using (var db = new ObjectDb("WorkingTimeRequest_GetRequestHistory"))
            {
                using (IDataReader reader = db.ExecuteReader(new { requestid = requestId, type = requestTypeId }))
                {
                    entityList = new List<RequestEntity>();
                    if (reader != null)
                    {
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
                ApprovedDate = reader.ToDateTime("ApprovedDate"),
                VerifiedDate = reader.ToDateTime("VerifiedDate"),
                HrDate = reader.ToDateTime("HrDate"),
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
