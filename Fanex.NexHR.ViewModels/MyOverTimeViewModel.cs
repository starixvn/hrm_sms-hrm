// --------------------------------------------------------------------------
// <copyright file="MyOverTimeViewModel.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Bradley.Mac" date="2014.8.6">Created</change>
// </history>

namespace Fanex.NexHR.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Text;
    using System.Web.Mvc;
    using Fanex.NexHR.Models.Overtime;

    /// <summary>
    /// My Overtime ViewModel.
    /// </summary>
    public class MyOverTimeViewModel
    {
        private const string _OverTimeRequesttDetailTpl = "<Request OnDate='{0}' ShiftID='{1}' TimeIn='{2}' "
            + "TimeOut='{3}' TotalHours='{4}' NormalHours='{5}' OffHours='{6}' PublicHours='{7}' Remark='{8}'></Request>";

        [Display(Name = "Verified by")]
        public List<SelectListItem> Verifier { get; set; }

        [Display(Name = "Approved by")]
        public List<SelectListItem> Approver { get; set; }

        public int? VerifierId { get; set; }

        public int? ApproverId { get; set; }

        public IList<OvertimeRequest> OvertimeRequest { get; set; }

        public int ProcessedBy { get; set; }

        public DateTime ProcessedDate { get; set; }

        public string OvertimeRequestStatus { get; set; }

        [Required]
        public string Employee { get; set; }

        [Required]
        public int EmpId { get; set; }

        public string OvertimeRequestDetail
        {
            get 
            {
                var value = new StringBuilder("<Requests>");

                if (OvertimeRequest != null)
                {
                    foreach (var detail in OvertimeRequest)
                    {
                        value.AppendFormat(
                            _OverTimeRequesttDetailTpl, 
                            detail.OvertimeDate.ToString("yyyyMMdd"), 
                            detail.Schedule, 
                            detail.TimeIn != null ? detail.TimeIn : string.Empty, 
                            detail.TimeOut != null ? detail.TimeOut : string.Empty, 
                            detail.TotalWorkHours, 
                            detail.NormalHours, 
                            detail.OffHours,
                            detail.PublicHours, 
                            detail.Remark);
                    }
                }

                return value.Append("</Requests>").ToString();
            }
        }
    }
}
