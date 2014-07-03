// --------------------------------------------------------------------------
// <copyright file="MemberScheduleOrderModel.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2014.7.3">Created</change>
// </history>

namespace Fanex.NexHR.Models
{
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// MemberScheduleOrder Model.
    /// </summary>
    public class MemberScheduleOrderModel
    {
        private const string _MemberOrderTpl = "<Employee EmployeeId='{0}' OrderCode='{1}'></Employee>";

        public int SupervisorId { get; set; }

        public IList<MemberOrderModel> MemberOrders { get; set; }

        public int ModifiedBy { get; set; }

        public string MemberOrderXML
        {
            get
            {
                var value = new StringBuilder("<ROOT>");

                if (MemberOrders != null)
                {
                    foreach (var member in MemberOrders)
                    {
                        value.AppendFormat(_MemberOrderTpl, member.EmployeeId, member.OrderCode);
                    }
                }

                return value.Append("</ROOT>").ToString();
            }
        }
    }
}
