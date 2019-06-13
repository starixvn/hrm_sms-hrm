namespace Fanex.NexHR.Controllers
{
    using System;
    using System.Web.Mvc;
    using Core.Models;
    using Core.Services;
    using Fanex.NexHR.Core.Constants;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Core.Filters;
    using Fanex.NexHR.Criteria.Employee;
    using Fanex.NexHR.Criteria.Message;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Services;

    public class MessageController : BaseController
    {
        private readonly IMessageService messageService;
        private readonly ITeamService teamService;
        private readonly IEmployeeService employeeService;
        private readonly IBaseService baseService;

        public MessageController(
            ISessionService sessionService,
            IMessageService messageService,
            ITeamService teamService,
            IEmployeeService employeeService,
            IBaseService baseService) : base(sessionService)
        {
            this.messageService = messageService;
            this.teamService = teamService;
            this.employeeService = employeeService;
            this.baseService = baseService;
        }

        [HRMPermission(Policy.Read, FunctionKey.Message)]
        public ActionResult Index()
        {
            var criteria = new GetMessageByCreatorCriteria
            {
                ToDate = DateTime.Now,
                FromDate = DateTime.Now.AddMonths(-3),
                IsDisabled = 0,
                CreatedBy = sessionService.Employee.Id
            };

            ViewBag.Messages = messageService.GetMessageByCreator(criteria);
            ViewBag.HasPermission = sessionService.HasPermissionOnFunction(FunctionKey.Message, Policy.Host);

            return View(criteria);
        }

        [HttpPost]
        [HRMPermission(Policy.Read, FunctionKey.Message)]
        public ActionResult Index(GetMessageByCreatorCriteria criteria)
        {
            if (criteria != null)
            {
                criteria.CreatedBy = sessionService.Employee.Id;
            }

            ViewBag.Messages = messageService.GetMessageByCreator(criteria);
            ViewBag.HasPermission = sessionService.HasPermissionOnFunction(FunctionKey.Message, Policy.Host);

            return View(criteria);
        }

        [HRMPermission(Policy.Host, FunctionKey.Message)]
        public ActionResult Create()
        {
            var getEmployeeCriteria = new GetAllEmployeeCriteria
            {
                Query = null,
                GroupIds = null,
                EnableUser = true,
                IsDeleteUser = null,
                TitleId = null,
                IsLocalUser = null
            };
            ViewBag.Employees = employeeService.GetEmployees(getEmployeeCriteria);
            ViewBag.Groups = teamService.GetAll();
            var model = new Message { FromDate = DateTime.Now, ToDate = DateTime.Now };

            return View(model);
        }

        [HttpPost]
        [HRMPermission(Policy.Host, FunctionKey.Message)]
        public JsonResult Create(Message model)
        {
            var result = new CommandResult();

            if (model != null && ModelState.IsValid)
            {
                var criteria = new InsertMessageCriteria(model)
                {
                    CreatedBy = sessionService.Employee.Id
                };

                result = baseService.ExecuteNonQuery(criteria);
                if (model.HasSendEmail && result.IsSuccessful)
                {
                    messageService.SendMailToStaff(model);
                }
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HRMPermission(Policy.Host, FunctionKey.Message)]
        public ActionResult Edit(int id)
        {
            var getMessageByIdCriteria = new GetMessageByIdCriteria
            {
                MessageId = id
            };

            ViewBag.Message = messageService.GetMessageById(getMessageByIdCriteria);
            var getEmployeeCriteria = new GetAllEmployeeCriteria
            {
                Query = null,
                GroupIds = null,
                EnableUser = true,
                IsDeleteUser = false,
                TitleId = null,
                IsLocalUser = null
            };

            ViewBag.Employees = employeeService.GetEmployees(getEmployeeCriteria);
            ViewBag.Groups = teamService.GetAll();

            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        [HRMPermission(Policy.Host, FunctionKey.Message)]
        public JsonResult Edit(Message model)
        {
            var result = new CommandResult();

            if (model != null && ModelState.IsValid)
            {
                var criteria = new UpdateMessageCriteria(model)
                {
                    ModifiedBy = sessionService.Employee.Id
                };

                result = baseService.ExecuteNonQuery(criteria);
                if (model.HasSendEmail && result.IsSuccessful)
                {
                    messageService.SendMailToStaff(model);
                }
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [HRMPermission(Policy.Host, FunctionKey.Message)]
        public JsonResult Delete(int id)
        {
            var criteria = new DeleteMessageCriteria
            {
                MessageId = id,
                ModifiedBy = sessionService.Employee.Id,
                ModifiedDate = DateTime.Now
            };
            CommandResult result = baseService.ExecuteNonQuery(criteria);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ViewMessageOfEmployee()
        {
            var criteria = new GetMessageByEmployeeCriteria
            {
                FromDate = DateTime.Today,
                ToDate = DateTime.Today,
                IsDisabled = 0,
                EmployeeId = sessionService.Employee.Id
            };

            ViewBag.Messages = messageService.GetMessageByEmployee(criteria);

            return View(criteria);
        }

        [HttpPost]
        public ActionResult ViewMessageOfEmployee(GetMessageByEmployeeCriteria criteria)
        {
            if (criteria != null)
            {
                criteria.EmployeeId = sessionService.Employee.Id;
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
                EmployeeId = sessionService.Employee.Id
            };
            CommandResult result = baseService.ExecuteNonQuery(criteria);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HRMPermission(Policy.Read, FunctionKey.Message)]
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
                EmployeeId = sessionService.Employee.Id
            };
            CommandResult result = baseService.ExecuteNonQuery(criteria);

            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}