namespace Fanex.NexHR.Services
{
    using System;
    using Fanex.NexHR.Models;

    public interface IRequestMailService
    {
        void SendMailForProcessSuccessfully(long requestId, int processedBy, string action);

        void SendMailForDeleteSuccessfully(long requestId);

        EmailModel GetMailToAndMailCC(long requestId, int action);

        string GetStringDateForLeaveRequest(DateTime requestedDate, DateTime fromDate, DateTime toDate, decimal totalDays);
    }
}