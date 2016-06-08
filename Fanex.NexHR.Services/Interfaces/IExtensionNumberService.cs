namespace Fanex.NexHR.Services.Interfaces
{
    using Fanex.NexHR.Criteria.ExtensionNumber;
    using Fanex.NexHR.Models;
    using System.Collections.Generic;

    public interface IExtensionNumberService
    {
        IList<ExtensionNoModel> GetAllExtensionNo(GetAllExtensionNumberCriteria criteria);

        IList<ExtensionNoModel> GetAllExtensionGroupInfo(GetExtensionNumberGroupInfoCriteria criteria);

        ExtensionNoModel GetEmployeeExtensionNoById(GetExtensionNumberMemberByIdCriteria criteria);
    }
}
