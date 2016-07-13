namespace Fanex.NexHR.Models
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class ScheduleArrangeModel
    {
        private const string _ScheduleModelTpl =
            "<WorkingCalenndar EmpId='{0}' WorkDate='{1}' ShiftId='{2}' IsHoliday='{3}' IsOffDay='{4}' IsPublic='{5}'></WorkingCalenndar>";

        private const string _ShiftMultipleModelTpl = "<ShiftMultiple EmpId='{0}' ForDate='{1}' ShiftId='{2}'></ShiftMultiple>";

        private const string _BeginRootTag = "<ROOT>";

        private const string _EndRootTag = "</ROOT>";

        public DateTime ModifiedDate { get; set; }

        public int ModifiedBy { get; set; }

        public IList<ScheduleModel> ScheduleModels { get; set; }

        public IList<ShiftMultipleModel> ShiftMultipleModels { get; set; }

        public string ScheduleModelXML
        {
            get
            {
                var value = new StringBuilder(_BeginRootTag);

                if (ScheduleModels != null)
                {
                    foreach (var schedule in ScheduleModels)
                    {
                        value.AppendFormat(_ScheduleModelTpl, schedule.EmployeeId, schedule.WorkDate, schedule.ShiftId, schedule.IsHoliday, schedule.IsOffDay, schedule.IsPublic);
                    }
                }

                return value.Append(_EndRootTag).ToString();
            }
        }

        public string ShiftMultipleXML
        {
            get
            {
                var value = new StringBuilder(_BeginRootTag);
                if (ShiftMultipleModels != null)
                {
                    foreach (var shiftMultiple in ShiftMultipleModels)
                    {
                        value.AppendFormat(_ShiftMultipleModelTpl, shiftMultiple.EmployeeId, shiftMultiple.ForDate, shiftMultiple.ShiftId);
                    }
                }

                return value.Append(_EndRootTag).ToString();
            }
        }
    }
}