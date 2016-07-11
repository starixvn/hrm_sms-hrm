namespace Fanex.NexHR.ViewModels.Request
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Models.Discipline;
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

        public RequestEntity CheckInOutDetail { get; set; }

        public RequestEntity LateInEarlyOutDetail { get; set; }

        public List<ChangeShiftDetailModel> ChangeShiftDetails { get; set; }

        public List<LeaveDetailModel> LeaveDetails { get; set; }

        public string IgnoredDisciplineDetails { get; set; }

        public List<IgnoredDisciplineModel> IgnoredDisciplines
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