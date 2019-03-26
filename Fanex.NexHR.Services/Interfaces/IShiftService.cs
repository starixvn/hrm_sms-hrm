namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Fanex.NexHR.Criteria.Shift;
    using Fanex.NexHR.Models;

    public interface IShiftService
    {
        IList<Shift> GetAll(int statusId = 1, bool isShowOff = false);

        IList<Shift> GetByQuery(string query, bool isShowOff);

        IList<SelectListItem> GetSelectList(int statusId = 1, bool isShowOff = false);

        Shift GetByDate(GetEmployeeShiftByDateCriteria criteria);

        Shift GetById(GetShiftByIdCriteria criteria);
    }
}