// -----------------------------------------------------------------------
// <copyright file="MessageController.cs" company="Nexcel Solution Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="2014.7.15">Created.</change>
// </history>

namespace Fanex.NexHR.Controllers
{
    using System.Web.Mvc;
    using Fanex.NexHR.Criteria.Message;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interface;

    /// <summary>
    /// The message controller.
    /// </summary>
    public class MessageController : BaseController
    {
        private readonly IMessageService _messageService;
        private readonly ILookupDataService _lookupDataService;

        public MessageController()
            : this(null, null)
        {
        }

        public MessageController(IMessageService messageService, ILookupDataService lookupDataService)
        {
            _messageService = messageService ?? new MessageService();
            _lookupDataService = lookupDataService ?? new LookupDataService();
        }

        public ActionResult Index()
        {
            var criteria = new GetMessageByCreatorCriteria
            {
                 CreatedBy = this.CurrentUser.Id
            };

            return Index(criteria);
        }

        [HttpPost]
        public ActionResult Index(GetMessageByCreatorCriteria criteria)
        {
            ViewBag.MemoCategories = _lookupDataService.GetAll(LookupCodeName.MemoCategory);
            ViewBag.Messages = _messageService.GetMessageByCreator(criteria);
            return View();
        }
    }
}
