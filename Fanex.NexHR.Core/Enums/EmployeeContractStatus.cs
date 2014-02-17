// -----------------------------------------------------------------------
// <copyright file="EmployeeContractStatus.cs" company="Nexcel Solutions Vietnam">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.NexHR.Core.Enums
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// ENUM Employee Contract Status.
    /// </summary>
    public enum EmployeeContractStatus
    {
        /// <summary>
        /// Default item.
        /// </summary>
        [StringValue("Default")]
        Default = 0,

        /// <summary>
        /// Probation item.
        /// </summary>
        [StringValue("Probation")]
        Probation = 1,

        /// <summary>
        /// Definite item.
        /// </summary>
        [StringValue("Definite")]
        Definite = 2,

        /// <summary>
        /// Indefinite item.
        /// </summary>
        [StringValue("Indefinite")]
        Indefinite = 3,
    }
}
