namespace Fanex.NexHR.Services
{
    using System;
    using System.Collections.Generic;
    using Fanex.NexHR.Core.Models;
    using Fanex.NexHR.Models.Request.ChangeShift;
    using Fanex.NexHR.Models.ScheduleArrange;

    public interface IChangeShiftRequestService
    {
        int GetNumberOfRequest(int employeeId, DateTime fromDate, DateTime toDate);

        IEnumerable<DailySchedule> GetWorkingCalendarForRequest(GetWorkingCalendarForRequestParam param);

        ChangeShiftRequest GetById(long requestId);

        IEnumerable<ChangeShiftDetail> GetDetailById(long requestId);

        CommandResult Create(int actorId, CreateChangeShift model);

        CommandResult Edit(int actorId, EditChangeShift model);

        CommandResult Delete(long requestId);

        void SendMailAfterCreateRequest(long requestId);

        void SendMailAfterEditRequest(long requestId);

        void SendMailAfterDeleteRequest(long requestId);
    }
}