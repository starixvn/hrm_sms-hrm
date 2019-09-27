namespace Fanex.NexHR.Services
{
    using System;
    using System.Collections.Generic;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Core.Models;
    using Fanex.NexHR.Models.Request;
    using Fanex.NexHR.Models.Request.ChangeShift;
    using Fanex.NexHR.Models.ScheduleArrange;

    public interface IChangeShiftRequestService
    {
        int GetNumberOfRequest(int employeeId, DateTime fromDate, DateTime toDate);

        IEnumerable<DailySchedule> GetWorkingCalendarForRequest(GetWorkingCalendarForRequestParam param);

        IEnumerable<DateTime> GetDisableDateForMoveOffDayRequest(int employeeId, DateTime requestDate, long? requestId = null);

        IEnumerable<ChangeShiftRequest> GetByEmployee(int employeeId, DateTime fromDate, DateTime toDate);

        IEnumerable<ChangeShiftRequest> GetHistoryById(long requestId);

        ChangeShiftRequest GetById(long requestId);

        IEnumerable<ChangeShiftDetail> GetDetailById(long requestId);

        CommandResult Create(int actorId, CreateChangeShift model);

        CommandResult Edit(int actorId, EditChangeShift model);

        CommandResult Delete(long requestId);

        CommandResult ApproveRequest(int actorId, ProcessRequestParam param);

        CommandResult ResetRequest(int actorId, ProcessRequestParam param);

        CommandResult RejectRequest(int actorId, ProcessRequestParam param);

        CommandResult VoidRequest(int actorId, ProcessRequestParam param);

        void SendMailAfterCreateRequest(long requestId);

        void SendMailAfterEditRequest(long requestId);

        void SendMailAfterDeleteRequest(long requestId);

        void SendMailAfterProcessRequest(long requestId, EmployeeSession actor, RequestCommand command);
    }
}