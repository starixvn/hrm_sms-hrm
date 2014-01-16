// --------------------------------------------------------------------------
// <copyright file="RequestViewType.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Paul.Nguyen" date="2013.11.21">Created</change>
// </history>

namespace Fanex.NexHR.Core.Enums
{
    public enum RequestViewType : byte
    {
        /// <summary>
        /// PersonalSpace item.
        /// </summary>
        [StringValue("PersonalSpace")]
        PersonalSpace = 1,

        /// <summary>
        /// HRSpace item.
        /// </summary>
        [StringValue("HrSpace")]
        HrSpace = 2,

        /// <summary>
        /// CompanySpace item.
        /// </summary>
        [StringValue("CompanySpace")]
        CompanySpace = 3
    }
}
