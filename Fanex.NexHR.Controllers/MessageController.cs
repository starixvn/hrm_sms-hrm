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
                ToDate = DateTime.Now,
                FromDate = DateTime.Now.AddMonths(-3),
                IsDisabled = 0
            };

            return Index(criteria);
        }

        [HttpPost]
        [Authorize(Roles = "HR")]
        public ActionResult Index(GetMessageByCreatorCriteria criteria)
        {
            if (criteria != null)
            {
                criteria.CreatedBy = this.CurrentUser.Id;
            }

            ViewBag.Messages = _messageService.GetMessageByCreator(criteria);
            return View(criteria);
        }

        /// <summary>
        /// Create Message.
        /// </summary>
        /// <returns>The Action Result.</returns>
        [Authorize(Roles = "HR")]
        public ActionResult Create()
        {
            ViewBag.Employees = _employeeService.GetEmployees(null, null, false, false, null, null);
            ViewBag.Groups = _teamService.GetAll();
            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        [Authorize(Roles = "HR")]
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

        /// <summary>
        /// Edit Message.
        /// </summary>
        /// <param name="id">The Message Id.</param>
        /// <returns>The Action Result.</returns>
        [Authorize(Roles = "HR")]
        public ActionResult Edit(int id)
        {
            var getMessageByIdCriteria = new GetMessageByIdCriteria
            {
                MessageId = id
            };

            ViewBag.Message = _messageService.GetMessageById(getMessageByIdCriteria);
            ViewBag.Employees = _employeeService.GetEmployees(null, null, false, false, null, null);
            ViewBag.Groups = _teamService.GetAll();

            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        [Authorize(Roles = "HR")]
        public JsonResult Edit(UpdateMessageByCreatorCriteria model)
        {
            if (model != null)
            {
                model.ModifiedBy = this.CurrentUser.Id;
                model.ModifiedDate = DateTime.Now;
            }

            CommandResult commandResult = _messageService.UpdateMessageByCreator(model);
            return Json(commandResult, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Delete Message.
        /// </summary>
        /// <param name="id">The Message Id.</param>
        /// <returns>The Command Result.</returns>
        [HttpPost]
        [Authorize(Roles = "HR")]
        public JsonResult Delete(int id)
        {
            var criteria = new DeleteMessageByCreatorCriteria
            {
                MessageId = id,
                ModifiedBy = this.CurrentUser.Id,
                ModifiedDate = DateTime.Now
            };

            CommandResult commandResult = _messageService.DeleteMessageByCreator(criteria);
            return Json(commandResult, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ViewMessageOfEmployee()
        {
            var criteria = new GetMessageByEmployeeCriteria
            {
                EffectedDate = DateTime.Today,
                IsDisabled = 0
            };

            return ViewMessageOfEmployee(criteria);
        }

        [HttpPost]
        public ActionResult ViewMessageOfEmployee(GetMessageByEmployeeCriteria criteria)
        {
            if (criteria != null)
            {
                criteria.EmployeeId = this.CurrentUser.Id;
            }

            ViewBag.Messages = _messageService.GetMessageByEmployee(criteria);
            return View(criteria);
        }

        [HttpPost]
        public JsonResult DeleteMessageByEmployee(int id)
        {
            var criteria = new DeleteMessageByEmployeeCriteria
            {
                MessageId = id,
                EmployeeId = this.CurrentUser.Id
            };

            CommandResult commandResult = _messageService.DeleteMessageByEmployee(criteria);
            return Json(commandResult, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "HR")]
        public ActionResult Detail(int id)
        {
            var getMessageByIdCriteria = new GetMessageByIdCriteria
            {
                MessageId = id
            };

            Message message = _messageService.GetMessageById(getMessageByIdCriteria);
            return View(message);
        }

        public ActionResult DetailByEmployee(int id)
        {
            var getMessageByIdCriteria = new GetMessageOfEmployeeByIdCriteria
            {
                MessageId = id
            };

            Message message = _messageService.GetDetailMessageByEmployee(getMessageByIdCriteria);
            return View(message);
        }

        public JsonResult UpdateReadStatusOfMessage(int id)
        {
            var criteria = new UpdateReadStatusMessageCriteria
            {
                MessageId = id,
                EmployeeId = this.CurrentUser.Id
            };

            CommandResult commandResult = _messageService.UpdateReadStatusOfMessage(criteria);
            return Json(commandResult, JsonRequestBehavior.AllowGet);
        }
    }
}
