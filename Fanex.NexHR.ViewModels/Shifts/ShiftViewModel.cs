namespace Fanex.NexHR.ViewModels.Shifts
{
    using System.ComponentModel.DataAnnotations;

    public class ShiftViewModel
    {
        public int ShiftId { get; set; }

        [Required]
        public string Name { get; set; }

        [DisplayFormat(DataFormatString = "{0:f2}", ApplyFormatInEditMode = true)]
        [Range(0, 23, ErrorMessage = "Hour is less than 23")]
        [Required]
        public int HourTimeIn { get; set; }

        [DisplayFormat(DataFormatString = "{0:f2}", ApplyFormatInEditMode = true)]
        [Range(0, 59, ErrorMessage = "Minutes is less than 59")]
        [Required]
        public int MinuteTimeIn { get; set; }

        [DisplayFormat(DataFormatString = "{0:f2}", ApplyFormatInEditMode = true)]
        [Range(0, 23, ErrorMessage = "Hour is less than 23")]
        [Required]
        public int HourTimeOut { get; set; }

        [DisplayFormat(DataFormatString = "{0:f2}", ApplyFormatInEditMode = true)]
        [Range(0, 59, ErrorMessage = "Minutes is less than 59")]
        [Required]
        public int MinuteTimeOut { get; set; }

        [Required]
        public bool IsInAfterDay { get; set; }

        [Required]
        public bool IsOutAfterDay { get; set; }

        public bool IsDisabled { get; set; }

        public bool IsUsed { get; set; }

        public string TimeIn
        {
            get
            {
                return HourTimeIn.ToString().PadLeft(2, '0') + ":" + MinuteTimeIn.ToString().PadLeft(2, '0');
            }
        }

        public string TimeOut
        {
            get
            {
                return HourTimeOut.ToString().PadLeft(2, '0') + ":" + MinuteTimeOut.ToString().PadLeft(2, '0');
            }
        }
    }
}
