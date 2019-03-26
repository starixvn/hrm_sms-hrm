namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using Core.Services;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Criteria.Shift;
    using Fanex.NexHR.Models;

    public class ShiftService : IShiftService
    {
        private readonly IBaseService baseService;

        public ShiftService(IBaseService baseService)
        {
            this.baseService = baseService;
        }

        public IList<Shift> GetAll(int statusId = 1, bool isShowOff = false)
        {
            IList<Shift> shifts = baseService.GetData<Shift>(new GetAllShiftCriteria()).ToList();

            if (statusId == (int)ShiftStatus.Active)
            {
                shifts = shifts.Where(t => !t.IsDisabled).ToList();
            }

            if (statusId == (int)ShiftStatus.Disabled)
            {
                shifts = shifts.Where(t => t.IsDisabled).ToList();
            }

            if (isShowOff)
            {
                shifts.Insert(0, new Shift { Code = "Off", Id = -1 });
            }

            return shifts;
        }

        public IList<Shift> GetByQuery(string query, bool isShowOff)
        {
            IList<Shift> shifts = GetAll(isShowOff: isShowOff);
            IList<Shift> queriedShifts = null;

            if (shifts != null && shifts.Any())
            {
                queriedShifts = shifts.Where(t => t.Code.ToUpperInvariant().StartsWith(query.ToUpperInvariant(), System.StringComparison.CurrentCulture)).ToList();
            }

            return queriedShifts ?? new List<Shift>();
        }

        public IList<SelectListItem> GetSelectList(int statusId = 1, bool isShowOff = false)
        {
            IList<SelectListItem> list = new List<SelectListItem>();
            IList<Shift> shifts = GetAll(statusId, isShowOff);

            if (shifts.Any())
            {
                list = shifts.Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToList();
            }

            return list;
        }

        public Shift GetByDate(GetEmployeeShiftByDateCriteria criteria)
        {
            return baseService.GetDataEntity<Shift>(criteria);
        }

        public Shift GetById(GetShiftByIdCriteria criteria)
        {
            return baseService.GetDataEntity<Shift>(criteria);
        }
    }
}