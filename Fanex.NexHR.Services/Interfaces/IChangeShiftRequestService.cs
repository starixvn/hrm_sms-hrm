namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using Fanex.NexHR.Criteria.Request.ChangeShift;
    using Fanex.NexHR.Models.Request;

    public interface IChangeShiftRequestService
    {
        IList<ChangeShiftDetailModel> GetChangeShiftRequestDetailById(GetChangeShiftRequestDetailByIdCriteria criteria);

        void SendMailForInsertSuccessfully(InsertChangeShiftRequestCriteria criteria);

        void SendMailForUpdateSuccessfully(UpdateChangeShiftRequestCriteria criteria);
    }
}