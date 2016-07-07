namespace Fanex.NexHR.ViewModels.Request
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Models.Discipline;
    using Fanex.NexHR.Models.Request;

    public class ProcessRequestViewModel
    {
        public ProcessRequestViewModel()
        {
            CheckInOutDetail = new RequestEntity();
            LateInEarlyOutDetail = new RequestEntity();
            ChangeShiftDetails = new List<ChangeShiftDetailModel>();
            LeaveDetails = new List<LeaveDetailModel>();
        }

        public long RequestId { get; set; }

        public int RoleId { get; set; }

        public int EmployeeId { get; set; }

        public string Command { get; set; }

        public int RequestTypeId { get; set; }

        [Display(Name = "Remark")]
        public string Comment { get; set; }

        public RequestEntity CheckInOutDetail { get; set; }

        public RequestEntity LateInEarlyOutDetail { get; set; }

        public IList<ChangeShiftDetailModel> ChangeShiftDetails { get; set; }

        public IList<LeaveDetailModel> LeaveDetails { get; set; }

        public string IgnoredDisciplineDetails { get; set; }

        public IList<IgnoredDisciplineModel> IgnoredDisciplines
        {
            get
            {
                if (IgnoredDisciplineDetails != null)
                {
                    return JsonConverter.FromJsonArray<IgnoredDisciplineModel>(IgnoredDisciplineDetails);
                }
                else
                {
                    return new List<IgnoredDisciplineModel>();
                }
            }
        }
    }
}