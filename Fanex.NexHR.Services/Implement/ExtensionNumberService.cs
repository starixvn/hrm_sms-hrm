namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using Core.Services;
    using Fanex.NexHR.Criteria.ExtensionNumber;
    using Fanex.NexHR.Models;

    public class ExtensionNumberService : IExtensionNumberService
    {
        private readonly IBaseService baseService;

        public ExtensionNumberService(IBaseService baseService)
        {
            this.baseService = baseService;
        }

        public IList<ExtensionNoModel> GetAllExtensionNo(GetAllExtensionNumberCriteria criteria)
        {
            return baseService.GetData<ExtensionNoModel>(criteria).ToList();
        }

        public IList<ExtensionNoModel> GetAllExtensionGroupInfo(GetExtensionNumberGroupInfoCriteria criteria)
        {
            return baseService.GetData<ExtensionNoModel>(criteria).ToList();
        }

        public ExtensionNoModel GetEmployeeExtensionNoById(GetExtensionNumberMemberByIdCriteria criteria)
        {
            return baseService.GetDataEntity<ExtensionNoModel>(criteria);
        }
    }
}