// -----------------------------------------------------------------------
// <copyright file="Country.cs" company="Nexcel Solutions Vietnam">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.NexHR.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Country Model.
    /// </summary>
    public class Country : EntityBase<int>
    {
        public string CountryCode { get; set; }

        public string CountryName { get; set; }

        public int CountryOrder { get; set; }
    }
}
