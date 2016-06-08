namespace Fanex.NexHR.Services
{
    using Fanex.Common.Helper;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Criteria.Shift;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Services.Interfaces;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;

    public class ShiftService : BaseService, IShiftService
    {
        public IList<Shift> GetAll(int statusId = 1, bool isShowOff = false)
        {
            var shifts = GetData<Shift>(new GetAllShiftCriteria()).ToList();
            if (statusId == (int)ShiftStatus.Active)
            {
                shifts = shifts.Where(t => !t.IsDisabled).ToList();
            }
            else if (statusId == (int)ShiftStatus.Disabled)
            {
                shifts = shifts.Where(t => t.IsDisabled).ToList();
            }

            if (isShowOff)
            {
                var offDay = new Shift
                {
                    Code = "Off",
                    Id = -1
                };

                shifts.Insert(0, offDay);
            }

            return shifts;
        }

        public IList<Shift> GetByQuery(string query, bool isShowOff)
        {
            IList<Shift> shifts = GetAll(isShowOff: isShowOff);
            query = query.ToUpper();

            if (shifts != null && shifts.Any())
            {
                shifts = shifts.Where(t => t.Code.ToUpper().StartsWith(query.ToUpper(), System.StringComparison.CurrentCulture)).ToList();
            }

            return shifts ?? new List<Shift>();
        }

        public IList<SelectListItem> GetSelectList(int statusId = 1, bool isShowOff = false)
        {
            IList<SelectListItem> list = new List<SelectListItem>();
            IList<Shift> shifts = GetAll(statusId, isShowOff);

            foreach (Shift item in shifts)
            {
                var listItem = new SelectListItem
                {
                    Value = item.Id.ToString(),
                    Text = item.Name
                };

                list.Add(listItem);
            }

            return list;
        }

        public Shift GetByDate(GetEmployeeShiftByDateCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            if (criteria.IsValid())
            {
                return GetDataEntity<Shift>(criteria);
            }

            return null;
        }

        public Shift GetById(GetShiftByIdCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            if (criteria.IsValid())
            {
                return GetDataEntity<Shift>(criteria);
            }

            return null;
        }
    }
}
