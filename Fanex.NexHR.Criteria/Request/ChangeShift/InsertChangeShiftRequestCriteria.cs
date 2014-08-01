// --------------------------------------------------------------------------
// <copyright file="InsertChangeShiftRequestCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2014.8.1">Created</change>
// </history>

namespace Fanex.NexHR.Criteria.Request.ChangeShift
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Fanex.Data.Repository;
    using Fanex.NexHR.Models.Request;

    /// <summary>
    /// InsertChangeShiftRequest Criteria.
    /// </summary>
    public class InsertChangeShiftRequestCriteria : CriteriaBase
    {
        private readonly string _changeShiftDetailTpl = "<Request OnDate='{0}' FromShift='{1}' ToShift='{2}' Remark='{3}'></Request>";

        public int EmpId { get; set; }

        public short RequestType { get; set; }

        public string Remark { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public DateTime CreatedDate { get; set; }

        public int CreatedBy { get; set; }

        public int VerifierId { get; set; }

        public int ApproverId { get; set; }

        [SpParam(Ignored = true)]
        public IList<ChangeShiftDetailModel> RequestDetail { get; set; }

        public string ListDetail
        {
            get
            {
                var value = new StringBuilder("<ROOT>");
                if (RequestDetail != null)
                {
                    foreach (ChangeShiftDetailModel item in RequestDetail)
                    {
                        value.AppendFormat(_changeShiftDetailTpl, item.OnDate, item.FromShift, item.ToShift, item.Remark);
                    }
                }

                return value.Append("</ROOT>").ToString();
            }
        }

        public override string GetSettingKey()
        {
            return "WorkingTimeRequest_Insert_ChangeShift";
        }

        public override bool IsValid()
        {
            return EmpId > 0 && FromDate <= ToDate;
        }
    }
}
