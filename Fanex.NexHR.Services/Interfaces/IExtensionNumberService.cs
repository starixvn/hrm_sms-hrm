// --------------------------------------------------------------------------
// <copyright file="IExtensionNumberService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2015.10.28">Created</change>
// </history>

namespace Fanex.NexHR.Services.Interfaces
{
    using System.Collections.Generic;
    using Fanex.NexHR.Criteria.ExtensionNumber;
    using Fanex.NexHR.Models;

    public interface IExtensionNumberService
    {
        IList<ExtensionNoModel> GetAllExtensionNo(GetAllExtensionNumberCriteria criteria);

        IList<ExtensionNoModel> GetAllExtensionGroupInfo(GetExtensionNumberGroupInfoCriteria criteria);

        CommandResult AddNewExtensionNoMember(InsertExtensionNumberCriteria criteria);

        ExtensionNoModel GetEmployeeExtensionNoById(GetExtensionNumberMemberByIdCriteria criteria);

        CommandResult UpdateExtensionNoMember(UpdateExtensionNumberMemberCriteria criteria);

        CommandResult DeleteExtensionNumber(DeleteExtensionNumberMemberCriteria criteria);

        CommandResult AddNewExternalGroup(InsertExternalGroupCriteria criteria);

        CommandResult EditExternalGroup(UpdateExternalNumberCriteria criteria);

        CommandResult DeleteExternalGroup(DeleteExternalGroupCriteria criteria);
    }
}
