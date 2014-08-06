// --------------------------------------------------------------------------
// <copyright file="InsertLeaveRequestCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2014.7.29">Created</change>
// </history>

namespace Fanex.NexHR.Criteria.Request.Leave
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Fanex.Data.Repository;
    using Fanex.NexHR.Models.Request;

    /// <summary>
    /// InsertLeaveRequest Criteria.
    /// </summary>
    public class InsertLeaveRequestCriteria : CriteriaBase
    {
        private const string _LeaveRequestDetailTpl = 
            "<LeaveRequestDetail RequestId='{0}' EmpId='{1}' OnDate='{2}' " 
            + "LeaveType='{3}' IsHalfDay='{4}' WorkType='{5}' FromTime='{6}' ToTime='{7}'></LeaveRequestDetail>";

        public long RequestId { get; set; }

        public int EmpId { get; set; }

        [SpParam(Ignored = true)]
        public string EmployeeName { get; set; }

        public short RequestType { get; set; }

        public string Remark { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public int LeaveTypeId { get; set; }

        [SpParam(Ignored = true)]
        public string LeaveTypeName { get; set; }

        public decimal TotalDays { get; set; }

        public DateTime CreatedDate { get; set; }

        public int CreatedBy { get; set; }

        public int? VerifierId { get; set; }

        public int? ApproverId { get; set; }

        public string RefNo { get; set; }

        [SpParam(Ignored = true)]
        public IList<LeaveDetailModel> RequestDetail { get; set; }

        public string ListDetail
        {
            get
            {
                var value = new StringBuilder("<ROOT>");

                if (RequestDetail != null)
                {
                    foreach (var detail in RequestDetail)
                    {
                        value.AppendFormat(
                            _LeaveRequestDetailTpl, 
                            0,
                            EmpId, 
                            detail.LeaveDate,
                            detail.WorkType == 2 ? detail.LeaveTypeId.ToString() : string.Empty, 
                            detail.IsHalfDay, 
                            detail.WorkType, 
                            detail.FromTime, 
                            detail.ToTime);
                    }
                }

                return value.Append("</ROOT>").ToString();
            }
        }

        public override string GetSettingKey()
        {
            return "WorkingTimeRequest_Insert_LeaveMultiRequest";
        }

        public override bool IsValid()
        {
            return EmpId > 0 && FromDate <= ToDate;
        }
    }
}
