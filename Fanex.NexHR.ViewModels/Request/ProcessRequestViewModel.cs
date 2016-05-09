namespace Fanex.NexHR.ViewModels.Request
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Fanex.NexHR.Models.Request;

    public class ProcessRequestViewModel
    {
        public long RequestId { get; set; }

        public int RoleId { get; set; }

        public int EmployeeId { get; set; }

        public string Command { get; set; }

        public int RequestTypeId { get; set; }

        [Display(Name = "Remark")]
        public string Comment { get; set; }

        public IList<RequestEntity> CheckInOutDetails { get; set; }

        public IList<RequestEntity> LateInEarlyOutDetails { get; set; }

        public IList<ChangeShiftDetailViewModel> ChangeShiftDetails { get; set; }

        public IList<LeaveDetailModel> LeaveDetails { get; set; }
    }
}