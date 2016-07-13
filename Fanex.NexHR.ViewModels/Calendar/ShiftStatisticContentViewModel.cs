namespace Fanex.NexHR.ViewModels
{
    using System;
    using System.Collections.Generic;
    using Fanex.NexHR.Models;

    public class ShiftStatisticContentViewModel
    {
        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public string GroupIds { get; set; }

        public bool IsShowWholeCompany { get; set; }

        public byte? IsLocal { get; set; }

        public IList<ShiftStatisticModel> ShiftStatisticModels { get; set; }
    }
}