namespace Fanex.NexHR.Controllers
{
    using Fanex.NexHR.Criteria.Employee;
    using Fanex.NexHR.Criteria.Message;
    using Fanex.NexHR.Criteria.Team;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Resources;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interfaces;
    using System;
    using System.Web.Mvc;

    public class MessageController : BaseController
    {
        private readonly IMessageService messageService;
        private readonly ITeamService teamService;
        private readonly IEmployeeService employeeService;
        private readonly ICommandService commandService;

        public MessageController()
            : this(null, null, null, null)
        {
        }

        public MessageController(
            IMessageService messageService,
            ITeamService teamService,
            IEmployeeService employeeService,
            ICommandService commandService)
        {
            this.messageService = messageService ?? new MessageService();
            this.teamService = teamService ?? new TeamService();
            this.employeeService = employeeService ?? new EmployeeService();
            this.commandService = commandService ?? new CommandService();
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
                criteria.CreatedBy = CurrentUser.Id;
            }

            ViewBag.Messages = messageService.GetMessageByCreator(criteria);
            return View(criteria);
        }

        [Authorize(Roles = "HR")]
        public ActionResult Create()
        {
            var getEmployeeCriteria = new GetAllEmployeeCriteria()
            {
                Query = null,
                GroupIds = null,
                EnableUser = false,
                IsDeleteUser = false,
                TitleId = null,
                IsLocalUser = null
            };
            ViewBag.Employees = employeeService.GetEmployees(getEmployeeCriteria);

            var getAllGroupCriteria = new GetAllGroupCriteria();
            ViewBag.Groups = teamService.GetAll(getAllGroupCriteria);

            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        [Authorize(Roles = "HR")]
        public JsonResult Create(Message model)
        {
            CommandResult result = new CommandResult();

            if (model != null && ModelState.IsValid)
            {
                InsertMessageCriteria criteria = new InsertMessageCriteria(model);
                criteria.CreatedBy = CurrentUser.Id;

                result = commandService.Insert(criteria);
                if (model.HasSendEmail && result.IsSuccessful)
                {
                    messageService.SendMailToStaff(model);
                }
            }
            else
            {
                result.Code = 1;
                result.Message = Messages.InvalidInputData;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "HR")]
        public ActionResult Edit(int id)
        {
            var getMessageByIdCriteria = new GetMessageByIdCriteria
            {
                MessageId = id
            };

            ViewBag.Message = messageService.GetMessageById(getMessageByIdCriteria);
            var getEmployeeCriteria = new GetAllEmployeeCriteria()
            {
                Query = null,
                GroupIds = null,
                EnableUser = false,
                IsDeleteUser = false,
                TitleId = null,
                IsLocalUser = null
            };

            ViewBag.Employees = employeeService.GetEmployees(getEmployeeCriteria);

            var getAllGroupCriteria = new GetAllGroupCriteria();
            ViewBag.Groups = teamService.GetAll(getAllGroupCriteria);

            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        [Authorize(Roles = "HR")]
        public JsonResult Edit(Message model)
        {
            CommandResult result = new CommandResult();

            if (model != null && ModelState.IsValid)
            {
                UpdateMessageCriteria criteria = new UpdateMessageCriteria(model);
                criteria.ModifiedBy = CurrentUser.Id;

                result = commandService.Update(criteria);
                if (model.HasSendEmail && result.IsSuccessful)
                {
                    messageService.SendMailToStaff(model);
                }
            }
            else
            {
                result.Code = 1;
                result.Message = Messages.InvalidInputData;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Authorize(Roles = "HR")]
        public JsonResult Delete(int id)
        {
            var criteria = new DeleteMessageCriteria
            {
                MessageId = id,
                ModifiedBy = CurrentUser.Id,
            };

            CommandResult result = commandService.Delete(criteria);
            return Json(result, JsonRequestBehavior.AllowGet);
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

            ViewBag.Messages = messageService.GetMessageByEmployee(criteria);
            return View(criteria);
        }

        [HttpPost]
        public JsonResult DeleteMessageByEmployee(int id)
        {
            var criteria = new DeleteMessageByEmployeeCriteria
            {
                MessageId = id,
                EmployeeId = CurrentUser.Id
            };

            CommandResult result = commandService.Delete(criteria);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "HR")]
        public ActionResult Detail(int id)
        {
            var getMessageByIdCriteria = new GetMessageByIdCriteria
            {
                MessageId = id
            };

            Message message = messageService.GetMessageById(getMessageByIdCriteria);
            return View(message);
        }

        public ActionResult DetailByEmployee(int id)
        {
            var getMessageByIdCriteria = new GetMessageOfEmployeeByIdCriteria
            {
                MessageId = id
            };

            Message message = messageService.GetDetailMessageByEmployee(getMessageByIdCriteria);
            return View(message);
        }

        public JsonResult UpdateReadStatusOfMessage(int id)
        {
            var criteria = new UpdateReadStatusMessageCriteria
            {
                MessageId = id,
                EmployeeId = CurrentUser.Id
            };

            CommandResult result = commandService.Update(criteria);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}
