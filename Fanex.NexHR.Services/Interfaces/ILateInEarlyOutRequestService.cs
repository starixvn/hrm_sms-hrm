namespace Fanex.NexHR.Services.Interfaces
{
    using Fanex.NexHR.Criteria.Request.LateInEarlyOut;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Request;

    public interface ILateInEarlyOutRequestService
    {
        EmployeeLateInEarlyOutModel GetActualLateInEarlyOut(GetActualLateInEarlyOutCriteria criteria);

        RequestEntity GetlateInEarlyOutRequestToIgnoreById(GetLateInEarlyOutRequestToIgnoreByIdCriteria criteria);

        void SendMailForInsertSuccessfully(InsertLateInEarlyOutRequestCriteria criteria);

        void SendMailForUpdateSuccessfully(UpdateLateInEarlyOutRequestCriteria criteria);
    }
}