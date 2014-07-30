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
    using System.Diagnostics.CodeAnalysis;
    using Fanex.Common.Extensions;

    [SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue", Justification = "Reviewed. Suppression is OK here.")]
    public enum RequestViewType : int
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
        HRSpace = 2,

        /// <summary>
        /// CompanySpace item.
        /// </summary>
        [StringValue("CompanySpace")]
        CompanySpace = 3
    }
}
