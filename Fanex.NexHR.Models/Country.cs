// --------------------------------------------------------------------------
// <copyright file="Country.cs" company=".">
// Copyright (c) .. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Bradley.Mac" date="2014.8.6">Created</change>
// </history>

namespace Fanex.NexHR.Models
{
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
