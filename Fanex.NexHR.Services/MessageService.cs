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
    using System.Web;
    using Fanex.Common.Helper;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Criteria.Message;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Services.Interface;
    using Fanex.NexHR.ViewModels;

    /// <summary>
    /// The implement of Message service.
    /// </summary>
    public class MessageService : BaseService, IMessageService
    {
        private readonly IEmailService _emailService;

        public MessageService()
            : this(null)
        {
        }

        public MessageService(IEmailService emailService)
        {
            _emailService = emailService ?? new EmailService();
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
                IEnumerable<Message> rawMessages = GetData<Message>(criteria);

                foreach (Message rawMessageItem in rawMessages)
                {
                    var messageItem = new MessageViewModel
                    {
                        MessageId = rawMessageItem.MessageId,
                        Subject = rawMessageItem.Subject,
                        CategoryName = GetCategoryName(rawMessageItem.Category),
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
                commandResult = _Repository.Insert(criteria);

                if (commandResult != null && commandResult.Code == 0)
                {
                    SendEmail(criteria);
                }
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

                if (message != null)
                {
                    message.CategoryName = GetCategoryName(message.Category);
                }
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

                foreach (Message message in messages)
                {
                    message.CategoryName = GetCategoryName(message.Category);
                }
            }

            return messages;
        }

        /// <summary>
        /// Delete Message by employee.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The Command Result.</returns>
        public CommandResult DeleteMessageByEmployee(DeleteMessageByEmployeeCriteria criteria)
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
        /// Get message category name.
        /// </summary>
        /// <param name="categoryId">The category Id.</param>
        /// <returns>The category name.</returns>
        private string GetCategoryName(int categoryId)
        {
            string category = string.Empty;

            switch (categoryId)
            {
                case (int)MessageCategory.Notification:
                    category = MessageCategory.Notification.ToString();
                    break;

                case (int)MessageCategory.Warning:
                    category = MessageCategory.Warning.ToString();
                    break;

                case (int)MessageCategory.Violation:
                    category = MessageCategory.Violation.ToString();
                    break;
            }

            return category;
        }

        /// <summary>
        /// Send email.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        private void SendEmail(InsertMessageCriteria criteria)
        {
            var bodyTemplate = Utilities.ReadTextFile(HttpContext.Current.Server.MapPath("~/app_data/emails/MessageNotification.cshtml"));

            var content = new
            {
                Subject = criteria.Subject,
                Content = criteria.Contents
            };
            var body = RazorEngine.Razor.Parse(bodyTemplate, content);

            _emailService.SendEmail(
                string.Empty,
                criteria.SendToEmail,
                string.Empty,
                Resources.Messages.MessageSubject + " " + criteria.Subject,
                body);
        }
    }
}
