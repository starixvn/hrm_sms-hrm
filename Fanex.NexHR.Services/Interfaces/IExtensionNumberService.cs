namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using Fanex.NexHR.Core.Models;
    using Fanex.NexHR.Models;

    public interface IExtensionNumberService
    {
        IEnumerable<ExtensionNoModel> GetAllExtensionNo();

        IList<ExtensionNoModel> GetAllExtensionGroupInfo(int? groupId = null, bool? isInternal = null);

        ExtensionNoModel GetEmployeeExtensionNoById(int employeeExtensionNoId, bool isInternal);

        CommandResult DeleteExtensionNumberById(int id);

        CommandResult CreateExternalGroup(string groupName);

        CommandResult EditExternalGroup(int groupId, string groupName);

        CommandResult DeleteExternalGroup(int groupId);
    }
}