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
        private readonly IMessageRepository _messageRepository;
        private readonly IBaseRepository _baseRepository;

        public MessageService()
            : this(null, null)
        {
        }

        public MessageService(IMessageRepository messageRepository, IBaseRepository baseRepository)
        {
            _messageRepository = messageRepository ?? new MessageRepository();
            _baseRepository = baseRepository ?? new BaseRepository();
        }

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
                IEnumerable<Message> rawMessages = _messageRepository.GetMessageByCreator(criteria);

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
                _baseRepository.Insert(criteria);
            }

            return commandResult;
        }
    }
}
