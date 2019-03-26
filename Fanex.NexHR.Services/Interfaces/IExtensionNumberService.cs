namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using Fanex.NexHR.Criteria.ExtensionNumber;
    using Fanex.NexHR.Models;

    public interface IExtensionNumberService
    {
        IList<ExtensionNoModel> GetAllExtensionNo(GetAllExtensionNumberCriteria criteria);

        IList<ExtensionNoModel> GetAllExtensionGroupInfo(GetExtensionNumberGroupInfoCriteria criteria);

        ExtensionNoModel GetEmployeeExtensionNoById(GetExtensionNumberMemberByIdCriteria criteria);
    }
}