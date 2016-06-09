namespace Fanex.NexHR.Services.Interfaces
{
    using Fanex.NexHR.Criteria.Request.ChangeShift;
    using Fanex.NexHR.Models.Request;
    using System.Collections.Generic;

    public interface IChangeShiftRequestService
    {
        IList<ChangeShiftDetailModel> GetChangeShiftRequestDetailById(GetChangeShiftRequestDetailByIdCriteria criteria);

        IList<ChangeShiftDetailModel> GetChangeShiftDetailToIgnoreById(GetChangeShiftDetailToIgnoreByIdCriteria criteria);

        void SendMailForInsertSuccessfully(InsertChangeShiftRequestCriteria criteria);

        void SendMailForUpdateSuccessfully(UpdateChangeShiftRequestCriteria criteria);
    }
}