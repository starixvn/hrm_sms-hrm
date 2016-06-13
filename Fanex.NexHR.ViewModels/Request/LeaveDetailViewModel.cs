namespace Fanex.NexHR.ViewModels.Request
{
    using System.Collections.Generic;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Request;

    public class LeaveDetailViewModel
    {
        public IList<LeaveDetailModel> LeaveRequestDetail { get; set; }

        public IList<LeaveType> LeaveTypeList { get; set; }
    }
}
