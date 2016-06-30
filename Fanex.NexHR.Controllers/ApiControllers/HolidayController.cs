// --------------------------------------------------------------------------
// <copyright file="HolidayController.cs" company=".">
// Copyright (c) .. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.16">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.Controllers.ApiControllers
{
    using Criteria.Holidays;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interfaces;
    using System.Collections.Generic;
    using System.Web.Http;

    /// <summary>
    /// Holiday Controller.
    /// </summary>
    public class HolidayController : ApiController
    {
        private readonly IHolidayService _holidayService;

        public HolidayController()
            : this(null)
        {
        }

        public HolidayController(IHolidayService holidayRepository = null)
        {
            _holidayService = holidayRepository ?? new HolidayService();
        }

        public IList<Holiday> GetHolidays(GetHolidaysCriteria criteria)
        {
            return _holidayService.GetByYear(criteria);
        }
    }
}
