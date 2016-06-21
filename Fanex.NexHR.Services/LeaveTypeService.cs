// --------------------------------------------------------------------------
// <copyright file="LeaveTypeService.cs" company=".">
// Copyright (c) .. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.16">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.Common.Helper;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Criteria.LeaveType;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Resources;
    using Fanex.NexHR.Services.Interfaces;

    /// <summary>
    /// LeaveType Service.
    /// </summary>
    public class LeaveTypeService : BaseService, ILeaveTypeService
    {
        /// <summary>
        /// Function to get all leave type.
        /// </summary>
        /// <param name="criteria">GetLeaveType Criteria.</param>
        /// <returns>List leave type.</returns>
        public IList<LeaveType> GetAll(GetLeaveTypeCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IList<LeaveType> leaveTypes = null;
            if (criteria.IsValid())
            {
                leaveTypes = GetData<LeaveType>(criteria).ToList();
                leaveTypes.Insert(0, new LeaveType { Id = Constants.GeneralLeave, Name = Labels.GeneralLeave });
            }

            return leaveTypes;
        }
    }
}
