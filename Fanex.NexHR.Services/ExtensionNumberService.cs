namespace Fanex.NexHR.Services
{
    using Fanex.NexHR.Criteria.ExtensionNumber;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Services.Interfaces;
    using System.Collections.Generic;
    using System.Linq;

    public class ExtensionNumberService : BaseService, IExtensionNumberService
    {
        public IList<ExtensionNoModel> GetAllExtensionNo(GetAllExtensionNumberCriteria criteria)
        {
            IList<ExtensionNoModel> extensionNumbers = GetData<ExtensionNoModel>(criteria).ToList();
            return extensionNumbers;
        }

        public IList<ExtensionNoModel> GetAllExtensionGroupInfo(GetExtensionNumberGroupInfoCriteria criteria)
        {
            IList<ExtensionNoModel> groupsInfo = GetData<ExtensionNoModel>(criteria).ToList();
            return groupsInfo;
        }

        public ExtensionNoModel GetEmployeeExtensionNoById(GetExtensionNumberMemberByIdCriteria criteria)
        {
            ExtensionNoModel extensionNumber = GetDataEntity<ExtensionNoModel>(criteria);
            return extensionNumber;
        }
    }
}
