namespace Fanex.NexHR.Criteria.Request.ChangeShift
{
    using Fanex.Data.Repository;
    using Fanex.NexHR.Models.Request;
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Web;

    public class ChangeShiftCommand : CriteriaBase
    {
        private readonly string _changeShiftDetailTpl = "<Request OnDate='{0}' FromShift='{1}' ToShift='{2}' Remark='{3}' ToDate='{4}'></Request>";

        public int EmpId { get; set; }

        public string Remark { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public byte ChangeShiftType { get; set; }

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
                        value.AppendFormat(_changeShiftDetailTpl, item.OnDate, item.FromShiftId, item.ToShiftId, HttpUtility.HtmlEncode(item.Remark), item.ToDate);
                    }
                }

                return value.Append("</ROOT>").ToString();
            }
        }
    }
}
