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
    using Fanex.NexHR.Repositories;
    using Fanex.NexHR.Repositories.Interfaces;
    using Fanex.NexHR.Services.Interface;
    using Fanex.NexHR.ViewModels;

    /// <summary>
    /// The implement of Message service.
    /// </summary>
    public class MessageService : IMessageService
    {
        private IMessageRepository _messageRepository;

        public MessageService()
            : this(null)
        {
        }

        public MessageService(IMessageRepository messageRepository)
        {
            _messageRepository = messageRepository ?? new MessageRepository();
        }

        /// <summary>
        /// Get message by creator.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The list Message view model.</returns>
        public IList<MessageViewModel> GetMessageByCreator(GetMessageByCreatorCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IList<MessageViewModel> messages = null;

            if (criteria.IsValid())
            {
                IEnumerable<Message> rawMessages = _messageRepository.GetMessageByCreator(criteria);

                foreach (Message rawMessageItem in rawMessages)
                {
                    var messageItem = new MessageViewModel
                    {
                        MessageId = rawMessageItem.MessageId,
                        Subject = rawMessageItem.Subject,
                        Category = rawMessageItem.Category,
                        CreatedDate  = rawMessageItem.CreatedDate,
                        SendTo = rawMessageItem.SentTo
                    };

                    messages.Add(messageItem);
                }
            }

            return messages;
        }
    }
}
