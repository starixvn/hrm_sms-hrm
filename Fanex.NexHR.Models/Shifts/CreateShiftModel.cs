namespace Fanex.NexHR.Models
{
    using System.ComponentModel.DataAnnotations;

    public class CreateShiftModel
    {
        private const short MaximumHours = 23;
        private const short MaximumMinutes = 59;

        [Required]
        public string Name { get; set; }

        [DisplayFormat(DataFormatString = "{0:f2}", ApplyFormatInEditMode = true)]
        [Range(0, MaximumHours, ErrorMessage = "Hour is less than 23")]
        [Required]
        public int HourTimeIn { get; set; }

        [DisplayFormat(DataFormatString = "{0:f2}", ApplyFormatInEditMode = true)]
        [Range(0, MaximumMinutes, ErrorMessage = "Minutes is less than 59")]
        [Required]
        public int MinuteTimeIn { get; set; }

        [DisplayFormat(DataFormatString = "{0:f2}", ApplyFormatInEditMode = true)]
        [Range(0, MaximumHours, ErrorMessage = "Hour is less than 23")]
        [Required]
        public int HourTimeOut { get; set; }

        [DisplayFormat(DataFormatString = "{0:f2}", ApplyFormatInEditMode = true)]
        [Range(0, MaximumMinutes, ErrorMessage = "Minutes is less than 59")]
        [Required]
        public int MinuteTimeOut { get; set; }

        [Required]
        public bool IsInAfterDay { get; set; }

        [Required]
        public bool IsOutAfterDay { get; set; }

        public string TimeIn => HourTimeIn.ToString().PadLeft(2, '0') + ":" + MinuteTimeIn.ToString().PadLeft(2, '0');

        public string TimeOut => HourTimeOut.ToString().PadLeft(2, '0') + ":" + MinuteTimeOut.ToString().PadLeft(2, '0');
    }
}