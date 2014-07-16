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
    using System;
    using System.Web.Mvc;
    using Fanex.NexHR.Criteria.Message;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interface;

    /// <summary>
    /// The message controller.
    /// </summary>
    public class MessageController : BaseController
    {
        private readonly IMessageService _messageService;
        private readonly ILookupDataService _lookupDataService;
        private readonly ITeamService _teamService;
        private readonly IEmployeeService _employeeService;

        public MessageController()
            : this(null, null, null, null)
        {
        }

        public MessageController(
            IMessageService messageService,
            ILookupDataService lookupDataService,
            ITeamService teamService,
            IEmployeeService employeeService)
        {
            _messageService = messageService ?? new MessageService();
            _lookupDataService = lookupDataService ?? new LookupDataService();
            _teamService = teamService ?? new TeamService();
            _employeeService = employeeService ?? new EmployeeService();
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
            ////ViewBag.MemoCategories = _lookupDataService.GetAll(LookupCodeName.MemoCategory);
            ViewBag.Messages = _messageService.GetMessageByCreator(criteria);
            return View();
        }

        /// <summary>
        /// Create Message.
        /// </summary>
        /// <returns>The Action Result.</returns>
        public ActionResult Create()
        {
            ViewBag.Employees = _employeeService.GetEmployees(null, null, null, null, null, null);
            ViewBag.Groups = _teamService.GetAll();
            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public JsonResult Create(InsertMessageCriteria model)
        {
            if (model != null)
            {
                model.CreatedBy = this.CurrentUser.Id;
                model.CreatedDate = DateTime.Now;
            }

            CommandResult commandResult = _messageService.InsertMessageByCreator(model);
            return Json(commandResult, JsonRequestBehavior.AllowGet);
        }
    }
}
