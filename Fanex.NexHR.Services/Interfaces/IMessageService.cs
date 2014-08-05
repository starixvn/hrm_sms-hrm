// -----------------------------------------------------------------------
// <copyright file="IMessageService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="2014.7.15">Created.</change>
// </history>

namespace Fanex.NexHR.Services.Interfaces
{
    using System.Collections.Generic;
    using Fanex.NexHR.Criteria.Message;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.ViewModels;

    /// <summary>
    /// The interface of Message service.
    /// </summary>
    public interface IMessageService
    {
        IList<MessageViewModel> GetMessageByCreator(GetMessageByCreatorCriteria criteria);

        CommandResult InsertMessageByCreator(InsertMessageCriteria criteria);

        CommandResult UpdateMessageByCreator(UpdateMessageByCreatorCriteria criteria);

        Message GetMessageById(GetMessageByIdCriteria criteria);

        CommandResult DeleteMessageByCreator(DeleteMessageByCreatorCriteria criteria);

        IEnumerable<Message> GetMessageByEmployee(GetMessageByEmployeeCriteria criteria);

        CommandResult DeleteMessageByEmployee(DeleteMessageByEmployeeCriteria criteria);

        CommandResult UpdateReadStatusOfMessage(UpdateReadStatusMessageCriteria criteria);

        Message GetDetailMessageByEmployee(GetMessageOfEmployeeByIdCriteria criteria);

        IEnumerable<Message> GetUnreadMessageOfEmployee(GetUnreadMessageOfEmployeeCriteria criteria);
    }
}
