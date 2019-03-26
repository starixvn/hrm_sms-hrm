namespace Fanex.NexHR.Services
{
    using Fanex.NexHR.Criteria.Request.LateInEarlyOut;
    using Fanex.NexHR.Models.Request;

    public interface ILateInEarlyOutRequestService
    {
        EmployeeLateInEarlyOutModel GetActualLateInEarlyOut(GetActualLateInEarlyOutCriteria criteria);

        void SendMailForInsertSuccessfully(InsertLateInEarlyOutRequestCriteria criteria);

        void SendMailForUpdateSuccessfully(UpdateLateInEarlyOutRequestCriteria criteria);
    }
}