namespace Fanex.NexHR.Controllers
{
    using System;
    using System.Web.Mvc;
    using Fanex.NexHR.Core.Constants;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Core.Filters;
    using Fanex.NexHR.Core.Models;
    using Fanex.NexHR.Core.Services;
    using Fanex.NexHR.Criteria.Employee;
    using Fanex.NexHR.Models.Message;
    using Fanex.NexHR.Services;

    public class MessageController : BaseController
    {
        private readonly IMessageService messageService;
        private readonly ICommonDataService commonDataService;
        private readonly IEmployeeService employeeService;

        public MessageController(
            ISessionService sessionService,
            IMessageService messageService,
            ICommonDataService commonDataService,
            IEmployeeService employeeService) : base(sessionService)
        {
            this.messageService = messageService;
            this.commonDataService = commonDataService;
            this.employeeService = employeeService;
        }

        [HRMPermission(Policy.Read, FunctionKey.Message)]
        public ActionResult Index()
        {
            var param = new GetMessageByCreatorParam
            {
                ToDate = DateTime.Now,
                FromDate = DateTime.Now.AddMonths(-3),
                IsDisabled = 0,
                CreatedBy = sessionService.Employee.Id
            };

            ViewBag.Messages = messageService.GetMessageByCreator(param);
            ViewBag.HasPermission = sessionService.HasPermissionOnFunction(FunctionKey.Message, Policy.Host);

            return View(param);
        }

        [HttpPost]
        [HRMPermission(Policy.Read, FunctionKey.Message)]
        public ActionResult Index(GetMessageByCreatorParam param)
        {
            if (param != null)
            {
                param.CreatedBy = sessionService.Employee.Id;
            }

            ViewBag.Messages = messageService.GetMessageByCreator(param);
            ViewBag.HasPermission = sessionService.HasPermissionOnFunction(FunctionKey.Message, Policy.Host);

            return View(param);
        }

        [HRMPermission(Policy.Host, FunctionKey.Message)]
        public ActionResult Create()
        {
            var criteria = new GetAllEmployeeCriteria
            {
                Query = null,
                TeamIds = null,
                EnableUser = true,
                IsDeleteUser = null,
                TitleId = null,
                IsLocalUser = null
            };
            ViewBag.Employees = employeeService.GetEmployees(criteria);
            ViewBag.Groups = commonDataService.GetAllTeam();
            var model = new Message { FromDate = DateTime.Now, ToDate = DateTime.Now };

            return View(model);
        }

        [HttpPost]
        [HRMPermission(Policy.Host, FunctionKey.Message)]
        public JsonResult Create(Message model)
        {
            var commandResult = new CommandResult();

            if (model != null && ModelState.IsValid)
            {
                commandResult = messageService.Create(sessionService.Employee.Id, model);
                if (model.HasSendEmail && commandResult.IsSuccessful)
                {
                    messageService.SendMailToStaff(model);
                }
            }

            return Json(commandResult, JsonRequestBehavior.AllowGet);
        }

        [HRMPermission(Policy.Host, FunctionKey.Message)]
        public ActionResult Edit(int id)
        {
            ViewBag.Message = messageService.GetMessageById(id);
            var criteria = new GetAllEmployeeCriteria
            {
                Query = null,
                TeamIds = null,
                EnableUser = true,
                IsDeleteUser = false,
                TitleId = null,
                IsLocalUser = null
            };

            ViewBag.Employees = employeeService.GetEmployees(criteria);
            ViewBag.Groups = commonDataService.GetAllTeam();

            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        [HRMPermission(Policy.Host, FunctionKey.Message)]
        public JsonResult Edit(Message model)
        {
            var commandResult = new CommandResult();

            if (model != null && ModelState.IsValid)
            {
                commandResult = messageService.Edit(sessionService.Employee.Id, model);
                if (model.HasSendEmail && commandResult.IsSuccessful)
                {
                    messageService.SendMailToStaff(model);
                }
            }

            return Json(commandResult, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [HRMPermission(Policy.Host, FunctionKey.Message)]
        public JsonResult Delete(int id)
        {
            CommandResult commandResult = messageService.Delete(sessionService.Employee.Id, id);

            return Json(commandResult, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ViewMessageOfEmployee()
        {
            var param = new GetMessageByEmployeeParam
            {
                FromDate = DateTime.Today,
                ToDate = DateTime.Today,
                IsDisabled = 0,
                EmployeeId = sessionService.Employee.Id
            };
            ViewBag.Messages = messageService.GetMessageByEmployee(param);

            return View(param);
        }

        [HttpPost]
        public ActionResult ViewMessageOfEmployee(GetMessageByEmployeeParam param)
        {
            if (param != null)
            {
                param.EmployeeId = sessionService.Employee.Id;
            }

            ViewBag.Messages = messageService.GetMessageByEmployee(param);

            return View(param);
        }

        [HttpPost]
        public JsonResult DeleteMessageByEmployee(int id)
        {
            CommandResult commandResult = messageService.DeleteMessageByEmployee(sessionService.Employee.Id, id);

            return Json(commandResult, JsonRequestBehavior.AllowGet);
        }

        [HRMPermission(Policy.Read, FunctionKey.Message)]
        public ActionResult Detail(int id)
        {
            Message message = messageService.GetMessageById(id);
            return View(message);
        }

        public ActionResult DetailByEmployee(int id)
        {
            Message message = messageService.GetDetailMessageByEmployee(id);
            return View(message);
        }

        public JsonResult EditMessageStatus(int id)
        {
            CommandResult commandResult = messageService.EditStatus(sessionService.Employee.Id, id);

            return Json(commandResult, JsonRequestBehavior.AllowGet);
        }
    }
}
