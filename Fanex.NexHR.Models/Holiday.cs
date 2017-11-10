namespace Fanex.NexHR.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class Holiday : EntityBase<int>
    {
        [RegularExpression(@"^[^><]*$", ErrorMessage = "Holiday name cannot start by <")]
        public string HolidayName { get; set; }

        [DataType(DataType.Date)]
        public DateTime FromDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime ToDate { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime ModifiedDate { get; set; }

        public int ModifiedBy { get; set; }
    }
}