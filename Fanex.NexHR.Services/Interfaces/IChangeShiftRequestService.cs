namespace Fanex.NexHR.Services
{
    using System;
    using System.Collections.Generic;
    using Fanex.NexHR.Core.Models;
    using Fanex.NexHR.Criteria.Request.ChangeShift;
    using Fanex.NexHR.Models.Request.ChangeShift;
    using Fanex.NexHR.Models.ScheduleArrange;

    public interface IChangeShiftRequestService
    {
        int GetNumberOfRequest(int employeeId, DateTime fromDate, DateTime toDate);

        IEnumerable<DailySchedule> GetWorkingCalendarForRequest(GetWorkingCalendarForRequestParam param);

        CommandResult Create(int actorId, CreateChangeShift model);

        CommandResult Edit(int actorId, EditChangeShift model);

        CommandResult Delete(long requestId);

        IList<ChangeShiftDetail> GetChangeShiftRequestDetailById(GetChangeShiftRequestDetailByIdCriteria criteria);

        void SendMailAfterCreateRequest(long requestId, CreateChangeShift model);

        void SendMailAfterEditRequest(EditChangeShift model);
    }
}