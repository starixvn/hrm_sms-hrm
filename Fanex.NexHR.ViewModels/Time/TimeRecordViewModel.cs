namespace Fanex.NexHR.ViewModels.Time
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Diagnostics.CodeAnalysis;

    [SuppressMessage("Major Code Smell", "S109:Magic numbers should not be used", Justification = "Pending")]
    public class TimeRecordViewModel
    {
        [Required]
        public int EmpId { get; set; }

        [Required]
        public string Employee { get; set; }

        public DateTime RecordTime { get; set; }

        public DateTime InsertDate { get; set; }

        [Required]
        public short InOutType { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MMM dd, yyyy}", ApplyFormatInEditMode = true)]
        public DateTime ForDate { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:f2}", ApplyFormatInEditMode = true)]
        [Range(0, 23, ErrorMessage = "Hour is less than 23")]
        public int Hour { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:f2}", ApplyFormatInEditMode = true)]
        [Range(0, 59, ErrorMessage = "Minutes is less than 59")]
        public int Minute { get; set; }
    }
}