// --------------------------------------------------------------------------
// <copyright file="ChangeShiftViewModel.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.15">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Text;
    using System.Web.Mvc;

    /// <summary>
    /// ChangeShift ViewModel.
    /// </summary>
    public class ChangeShiftViewModel : BaseRequestModel
    {
        private readonly string _shiftRequests = "<Request><OnDate>{0}</OnDate><FromShift>{1}</FromShift><ToShift>{2}</ToShift><Remark>{3}</Remark></Request>";

        public long RequestId { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime ForDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime FromDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime ToDate { get; set; }

        [Required]
        public int FromShift1 { get; set; }

        public string FromShiftName1 { get; set; }

        [Required]
        public int ToShift { get; set; }

        public string ToShiftName { get; set; }

        public bool IsPlanned { get; set; }

        public List<ShiftDetailsViewModel> ShiftDetails { get; set; }

        [Display(Name = "Verified by")]
        public List<SelectListItem> Verifier { get; set; }

        [Display(Name = "Approved by")]
        public List<SelectListItem> Approver { get; set; }

        public int? VerifierId { get; set; }

        public int? ApproverId { get; set; }

        public int? RequestType { get; set; }

        public string Value
        {
            get
            {
                var value = new StringBuilder("<Requests>");

                if (ShiftDetails != null)
                {
                    foreach (var item in ShiftDetails)
                    {
                        value.AppendFormat(_shiftRequests, Convert.ToDateTime(item.Date).ToString("MM/dd/yyyy"), item.FromShift, item.ToShift, item.Remark); // 1 -> read
                    }
                }

                return value.Append("</Requests>").ToString();
            }
        }

        public string FormattedStatus { get; set; }
    }
}
