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
    using Fanex.Data.Repository;

    /// <summary>
    /// Update Order Shift Arrange Criteria.
    /// </summary>
    public class UpdateOrderShiftArrangeCriteria : CriteriaBase
    {
        public int SupervisorId { get; set; }

        public string EmployeeOrders { get; set; }

        public int ModifiedBy { get; set; }

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
