// -----------------------------------------------------------------------
// <copyright file="Country.cs" company="Nexcel Solutions Vietnam">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

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
