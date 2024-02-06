namespace Fanex.NexHR.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Core.Models;
    using Fanex.NexHR.Core.Services;
    using Fanex.NexHR.Criteria.Shift;
    using Fanex.NexHR.Models;

    public class ShiftService : IShiftService
    {
        private readonly IBaseService baseService;

        public ShiftService(IBaseService baseService)
        {
            this.baseService = baseService;
        }

        public IList<Shift> GetAll(byte statusId = 1, bool isShowOff = false)
        {
            var shifts = baseService.GetData<Shift>(new GetAllShiftCriteria()).ToList();

            if (statusId == (byte)ShiftStatus.Active)
            {
                shifts = shifts.Where(t => !t.IsDisabled).ToList();
            }

            if (statusId == (byte)ShiftStatus.Disabled)
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
                queriedShifts = shifts.Where(t => t.Code.ToUpperInvariant().StartsWith(query.ToUpperInvariant(), StringComparison.CurrentCulture)).ToList();
            }

            return queriedShifts ?? new List<Shift>();
        }

        public Shift GetByDate(int employeeId, DateTime date)
        {
            var criteria = new GetEmployeeShiftByDateCriteria
            {
                EmployeeId = employeeId,
                Date = date
            };
            return baseService.GetDataEntity<Shift>(criteria);
        }

        public Shift GetById(int shiftId)
        {
            var criteria = new GetShiftByIdCriteria { ShiftId = shiftId };
            return baseService.GetDataEntity<Shift>(criteria);
        }

        public CommandResult Create(int actorId, CreateShiftModel model)
        {
            var criteria = new CreateShiftCriteria
            {
                Name = model.Name,
                CheckInTime = model.TimeIn,
                CheckOutTime = model.TimeOut,
                CheckInDateType = (byte)model.CheckInDateType,
                CheckOutDateType = (byte)model.CheckOutDateType,
                ActorId = actorId
            };
            return baseService.ExecuteNonQuery(criteria);
        }

        public CommandResult Edit(int actorId, EditShiftModel model)
        {
            var criteria = new EditShiftCriteria
            {
                ShiftId = model.ShiftId,
                Name = model.Name,
                CheckInTime = model.TimeIn,
                CheckOutTime = model.TimeOut,
                CheckInDateType = (byte)model.CheckInDateType,
                CheckOutDateType = (byte)model.CheckOutDateType,
                IsDisabled = model.IsDisabled,
                ActorId = actorId
            };
            return baseService.ExecuteNonQuery(criteria);
        }

        public CommandResult Delete(int shiftId)
        {
            var criteria = new DeleteShiftCriteria { ShiftId = shiftId };
            return baseService.ExecuteNonQuery(criteria);
        }
    }
}