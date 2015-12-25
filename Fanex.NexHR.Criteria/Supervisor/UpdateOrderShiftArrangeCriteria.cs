// --------------------------------------------------------------------------
// <copyright file="UpdateOrderShiftArrangeCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Bradley.Mac" date="2014.7.30">Created</change>
// </history>

namespace Fanex.NexHR.Criteria.Supervisor
{
    using System.Collections.Generic;
    using System.Text;
    using Fanex.Data.Repository;
    using Fanex.NexHR.Models;

    public class UpdateOrderShiftArrangeCriteria : CriteriaBase
    {
        private const string _MemberOrderTpl = "<Employee EmployeeId='{0}' OrderCode='{1}' ></Employee>";

        public int SupervisorId { get; set; }

        [SpParam(Ignored = true)]
        public IList<MemberOrderModel> MemberOrders { get; set; }

        public string MemberOrderXml
        {
            get
            {
                var value = new StringBuilder("<ROOT>");

                if (MemberOrders != null)
                {
                    foreach (MemberOrderModel member in MemberOrders)
                    {
                        value.AppendFormat(_MemberOrderTpl, member.EmployeeId, member.OrderCode);
                    }
                }

                return value.Append("</ROOT>").ToString();
            }
        }

        public override string GetSettingKey()
        {
            return "EmployeeSupervisor_UpdateOrderShiftArrange";
        }

        public override bool IsValid()
        {
            return SupervisorId > 0;
        }
    }
}
