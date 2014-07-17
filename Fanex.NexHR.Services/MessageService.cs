// -----------------------------------------------------------------------
// <copyright file="MessageService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="2014.7.15">Created.</change>
// </history>

namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using Fanex.Common.Helper;
    using Fanex.NexHR.Criteria.Message;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Services.Interface;
    using Fanex.NexHR.ViewModels;

    /// <summary>
    /// The implement of Message service.
    /// </summary>
    public class MessageService : BaseService, IMessageService
    {
        /// <summary>
        /// Get message by creator.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The list Message view model.</returns>
        public IList<MessageViewModel> GetMessageByCreator(GetMessageByCreatorCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IList<MessageViewModel> messages = new List<MessageViewModel>();

            if (criteria.IsValid())
            {
                IEnumerable<Message> rawMessages = GetData<Message>(criteria);

                foreach (Message rawMessageItem in rawMessages)
                {
                    var messageItem = new MessageViewModel
                    {
                        MessageId = rawMessageItem.MessageId,
                        Subject = rawMessageItem.Subject,
                        Category = rawMessageItem.Category,
                        CreatedDate  = rawMessageItem.CreatedDate,
                        SendTo = rawMessageItem.SendTo
                    };

                    messages.Add(messageItem);
                }
            }

            return messages;
        }

        /// <summary>
        /// Insert new Message.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The Command Result.</returns>
        public CommandResult InsertMessageByCreator(InsertMessageCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            CommandResult commandResult = new CommandResult();

            if (criteria.IsValid())
            {
                _Repository.Insert(criteria);
            }

            return commandResult;
        }

        /// <summary>
        /// Update Message.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The Command Result.</returns>
        public CommandResult UpdateMessageByCreator(UpdateMessageByCreatorCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            CommandResult commandResult = new CommandResult();

            if (criteria.IsValid())
            {
                commandResult = _Repository.Update(criteria);
            }

            return commandResult;
        }

        /// <summary>
        /// Get Message by Id.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The Message model.</returns>
        public Message GetMessageById(GetMessageByIdCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            var message = new Message();

            if (criteria.IsValid())
            {
                message = GetDataEntity<Message>(criteria);
            }

            return message;
        }

        /// <summary>
        /// Delete Message by creator.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The Command Result.</returns>
        public CommandResult DeleteMessageByCreator(DeleteMessageByCreatorCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            CommandResult commandResult = new CommandResult();

            if (criteria.IsValid())
            {
                commandResult = _Repository.Delete(criteria);
            }

            return commandResult;
        }

        /// <summary>
        /// Get Message by employee.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The enumerator Message.</returns>
        public IEnumerable<Message> GetMessageByEmployee(GetMessageByEmployeeCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IEnumerable<Message> messages = null;

            if (criteria.IsValid())
            {
                messages = GetData<Message>(criteria);
            }

            return messages;
        }
    }
}
