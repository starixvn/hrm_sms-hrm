// --------------------------------------------------------------------------
// <copyright file="ExtensionNumberService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2015.10.28">Created</change>
// </history>

namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.Common.Helper;
    using Fanex.NexHR.Criteria.ExtensionNumber;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Services.Interfaces;

    public class ExtensionNumberService : BaseService, IExtensionNumberService
    {
        public IList<ExtensionNoModel> GetAllExtensionNo(GetAllExtensionNumberCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IList<ExtensionNoModel> extensionNumbers = null;

            if (criteria.IsValid())
            {
                extensionNumbers = GetData<ExtensionNoModel>(criteria).ToList();
            }

            return extensionNumbers;
        }

        public IList<ExtensionNoModel> GetAllExtensionGroupInfo(GetExtensionNumberGroupInfoCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IList<ExtensionNoModel> groupsInfo = null;

            if (criteria.IsValid())
            {
                groupsInfo = GetData<ExtensionNoModel>(criteria).ToList();
            }

            return groupsInfo;
        }

        public CommandResult AddNewExtensionNoMember(InsertExtensionNumberCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            CommandResult commandResult = new CommandResult();

            if (criteria.IsValid())
            {
                commandResult = Repository.Insert(criteria);
            }

            return commandResult;
        }

        public ExtensionNoModel GetEmployeeExtensionNoById(GetExtensionNumberMemberByIdCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            ExtensionNoModel extensionNumber = new ExtensionNoModel();

            if (criteria.IsValid())
            {
                extensionNumber = GetDataEntity<ExtensionNoModel>(criteria);
            }

            return extensionNumber;
        }

        public CommandResult UpdateExtensionNoMember(UpdateExtensionNumberMemberCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            CommandResult commandResult = new CommandResult();

            if (criteria.IsValid())
            {
                commandResult = Repository.Update(criteria);
            }

            return commandResult;
        }

        public CommandResult DeleteExtensionNumber(DeleteExtensionNumberMemberCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            CommandResult commandResult = new CommandResult();

            if (criteria.IsValid())
            {
                commandResult = Repository.Delete(criteria);
            }

            return commandResult;
        }

        public CommandResult AddNewExternalGroup(InsertExternalGroupCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            CommandResult commandResult = new CommandResult();

            if (criteria.IsValid())
            {
                commandResult = Repository.Insert(criteria);
            }

            return commandResult;
        }

        public CommandResult EditExternalGroup(UpdateExternalNumberCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            CommandResult commandResult = new CommandResult();

            if (criteria.IsValid())
            {
                commandResult = Repository.Update(criteria);
            }

            return commandResult;
        }

        public CommandResult DeleteExternalGroup(DeleteExternalGroupCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            CommandResult commandResult = new CommandResult();

            if (criteria.IsValid())
            {
                commandResult = Repository.Delete(criteria);
            }

            return commandResult;
        }
    }
}
