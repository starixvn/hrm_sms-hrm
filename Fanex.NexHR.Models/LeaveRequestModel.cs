// --------------------------------------------------------------------------
// <copyright file="LeaveRequestModel.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.11.11">Created</change>
// </history>

namespace Fanex.NexHR.Models
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Fanex.NexHR.Models.Request;

    /// <summary>
    /// LeaveRequest Model.
    /// </summary>
    public class LeaveRequestModel
    {
        private const string _LeaveRequestDetailTpl = "<LeaveRequestDetail RequestId='{0}' EmpId='{1}' OnDate='{2}' "
            + "LeaveType='{3}' IsHalfDay='{4}' WorkType='{5}' FromTime='{6}' ToTime='{7}'></LeaveRequestDetail>";

        public long RequestId { get; set; }

        public int EmployeeId { get; set; }

        public string EmployeeName { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public int LeaveTypeId { get; set; }

        public string LeaveTypeName { get; set; }

        public decimal TotalDays { get; set; }

        public string RefNo { get; set; }

        public string Remark { get; set; }

        public int? VerifierId { get; set; }

        public int? ApproverId { get; set; }

        public int CreatedBy { get; set; }

        public int ModifiedBy { get; set; }

        public IList<LeaveDetailModel> RequestDetail { get; set; }

        public string RequestDetailXML
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
                            RequestId, 
                            EmployeeId, 
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
    }
}
