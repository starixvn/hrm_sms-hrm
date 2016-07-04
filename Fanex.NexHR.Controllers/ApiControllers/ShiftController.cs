namespace Fanex.NexHR.Controllers.ApiControllers
{
    using System;
    using System.Web.Http;
    using Fanex.NexHR.Criteria.Shift;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interfaces;

    public class ShiftController : ApiController
    {
        private readonly IShiftService shiftService;

        public ShiftController() : this(null)
        {
        }

        public ShiftController(IShiftService shiftService = null)
        {
            this.shiftService = shiftService ?? new ShiftService();
        }

        public Shift GetShiftByDate(int employeeId, DateTime date)
        {
            var criteria = new GetEmployeeShiftByDateCriteria()
            {
                EmployeeId = employeeId,
                Date = date
            };

            Shift shift = shiftService.GetByDate(criteria);
            return shift;
        }
    }
}
