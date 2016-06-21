// --------------------------------------------------------------------------
// <copyright file="Command.cs" company=".">
// Copyright (c) .. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.16">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.Core.Enums
{
    using System.Diagnostics.CodeAnalysis;
    using Fanex.Common.Extensions;

    [SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue", Justification = "Reviewed. Suppression is OK here.")]
    public enum Command
    {
        /// <summary>
        /// Accept item.
        /// </summary>
        [StringValue("Accept")]
        Accept = 1,

        /// <summary>
        /// NotAccept item.
        /// </summary>
        [StringValue("NotAccept")]
        NotAccept = 2,

        /// <summary>
        /// Approve item.
        /// </summary>
        [StringValue("Approve")]
        Approve = 3,

        /// <summary>
        /// Reject item.
        /// </summary>
        [StringValue("Reject")]
        Reject = 4,

        /// <summary>
        /// Process item.
        /// </summary>
        [StringValue("Process")]
        Process = 5,

        /// <summary>
        /// Void item.
        /// </summary>
        [StringValue("Void")]
        Void = 6,

        /// <summary>
        /// Reset item.
        /// </summary>
        [StringValue("Reset")]
        Reset = 7
    }
}
