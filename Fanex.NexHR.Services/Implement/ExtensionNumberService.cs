namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using Core.Services;
    using Fanex.NexHR.Core.Models;
    using Fanex.NexHR.Criteria.ExtensionNumber;
    using Fanex.NexHR.Models;

    public class ExtensionNumberService : IExtensionNumberService
    {
        private readonly IBaseService baseService;

        public ExtensionNumberService(IBaseService baseService)
        {
            this.baseService = baseService;
        }

        public IEnumerable<ExtensionNoModel> GetAllExtensionNo()
        {
            var criteria = new GetAllExtensionNumberCriteria();
            return baseService.GetData<ExtensionNoModel>(criteria);
        }

        public IList<ExtensionNoModel> GetAllExtensionGroupInfo(int? groupId = null, bool? isInternal = null)
        {
            var criteria = new GetExtensionNumberGroupInfoCriteria
            {
                GroupId = groupId,
                IsInternal = isInternal
            };
            return baseService.GetData<ExtensionNoModel>(criteria).ToList();
        }

        public ExtensionNoModel GetEmployeeExtensionNoById(int employeeExtensionNoId, bool isInternal)
        {
            var criteria = new GetExtensionNumberMemberByIdCriteria
            {
                EmployeeExtensionNoId = employeeExtensionNoId,
                IsInternal = isInternal
            };
            return baseService.GetDataEntity<ExtensionNoModel>(criteria);
        }

        public CommandResult DeleteExtensionNumber(int extensionNoId)
        {
            var criteria = new DeleteExtensionNumberMemberCriteria { ExtensionNumberId = extensionNoId };
            return baseService.ExecuteNonQuery(criteria);
        }

        public CommandResult CreateExternalGroup(string groupName)
        {
            var criteria = new CreateExternalGroupCriteria
            {
                FullName = groupName
            };
            return baseService.ExecuteNonQuery(criteria);
        }

        public CommandResult EditExternalGroup(int groupId, string groupName)
        {
            var criteria = new UpdateExternalNumberCriteria
            {
                ExternalId = groupId,
                FullName = groupName
            };
            return baseService.ExecuteNonQuery(criteria);
        }

        public CommandResult DeleteExternalGroup(int groupId)
        {
            var criteria = new DeleteExternalGroupCriteria { ExternalId = groupId };
            return baseService.ExecuteNonQuery(criteria);
        }
    }
}