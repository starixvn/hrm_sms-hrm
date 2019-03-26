namespace Fanex.NexHR.Controllers
{
    using System;
    using System.Configuration;
    using System.Web.Mvc;
    using Core.Models;
    using Core.Services;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Constants;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Criteria.Lookup;
    using Fanex.NexHR.Criteria.Memo;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.ViewModels.Memo;
    using Fanex.NexHR.Web.Providers;
    using Services;

    public class MemoController : BaseController
    {
        private const int UploadMemoType = 1;
        private readonly IMemoService memoService;
        private readonly ILookupDataService lookupDataService;
        private readonly IEmailService emailService;
        private readonly ICommandService commandService;

        public MemoController(
            IMemoService memoService,
            ILookupDataService lookupDataService,
            IEmailService emailService,
            ICommandService commandService)
        {
            this.memoService = memoService;
            this.lookupDataService = lookupDataService;
            this.emailService = emailService;
            this.commandService = commandService;
        }

        [HRAuthorize(Policy.Read, FunctionKey.Memo)]
        public ActionResult Index()
        {
            var getAllMemoCriteria = new GetAllLookupDataCriteria { LookupCode = Constants.MemoCategory };
            var model = new MemoViewModel();
            var criteria = new GetAllMemoCriteria { Year = model.Year, Category = model.Category };

            model.MemoCategories = lookupDataService.GetAll(getAllMemoCriteria);
            model.Memos = memoService.GetAll(criteria);

            ViewBag.HasPermission = CurrentUser.HasPermission(FunctionKey.Memo, Policy.Host);

            return View(model);
        }

        [HttpPost]
        [HRAuthorize(Policy.Read, FunctionKey.Memo)]
        public ActionResult Index(MemoViewModel model)
        {
            var getAllMemoCriteria = new GetAllLookupDataCriteria { LookupCode = Constants.MemoCategory };
            var criteria = new GetAllMemoCriteria { Year = model.Year, Category = model.Category };

            model.MemoCategories = lookupDataService.GetAll(getAllMemoCriteria);
            model.Memos = memoService.GetAll(criteria);

            ViewBag.HasPermission = CurrentUser.HasPermission(FunctionKey.Memo, Policy.Host);

            return View(model);
        }

        [HttpGet]
        [HRAuthorize(Policy.Host, FunctionKey.Memo)]
        public ActionResult Edit(int id)
        {
            var getAllMemoCriteria = new GetAllLookupDataCriteria { LookupCode = Constants.MemoCategory };
            var getMemoById = new GetMemoByIdCriteria { Id = id };
            var memo = memoService.GetById(getMemoById);

            ViewBag.MemoCategories = lookupDataService.GetAll(getAllMemoCriteria);
            ViewBag.Updated = false;

            return View(memo);
        }

        [HttpPost]
        [ValidateInput(false)]
        [HRAuthorize(Policy.Host, FunctionKey.Memo)]
        public ActionResult Edit(Memo model)
        {
            var getAllMemoCriteria = new GetAllLookupDataCriteria { LookupCode = Constants.MemoCategory };
            ViewBag.MemoCategories = lookupDataService.GetAll(getAllMemoCriteria);
            if (model.MemoType == UploadMemoType && string.IsNullOrEmpty(model.AttachFile))
            {
                ViewBag.Updated = false;
                return View(model);
            }

            model.Contents = model.MemoType == UploadMemoType ? model.AttachFile : model.Contents;

            if (ModelState.IsValid)
            {
                var updateMemoCriteria = new UpdateMemoCriteria
                {
                    Id = model.Id,
                    RefNo = model.RefNo,
                    Contents = model.Contents,
                    Subject = model.Subject,
                    SentTo = model.SentTo,
                    SentFrom = model.SentFrom,
                    ForInfo = model.ForInfo,
                    TotalPages = model.TotalPages,
                    SentDate = model.SentDate,
                    Category = model.Category,
                    MemoType = model.MemoType
                };

                CommandResult result = commandService.Update(updateMemoCriteria);
                ViewBag.Updated = result.IsSuccessful;

                if (result.IsSuccessful && updateMemoCriteria.SendMail)
                {
                    SendMail(updateMemoCriteria.Subject, result);
                }

                if (!result.IsSuccessful)
                {
                    ModelState.AddModelError(string.Empty, result.Message);
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, Resources.Messages.InvalidInputData);
            }

            return View(model);
        }

        [HRAuthorize(Policy.Host, FunctionKey.Memo)]
        public ActionResult Create()
        {
            var getAllMemoCriteria = new GetAllLookupDataCriteria { LookupCode = Constants.MemoCategory };
            var memo = new Memo
            {
                SentTo = "All Staff",
                SentFrom = "Management",
                TotalPages = 1,
                SentDate = DateTime.Now,
                Contents = "[Contents]",
                MemoType = UploadMemoType
            };

            ViewBag.MemoCategories = lookupDataService.GetAll(getAllMemoCriteria);
            ViewBag.Updated = false;

            return View(memo);
        }

        [HttpPost]
        [ValidateInput(false)]
        [HRAuthorize(Policy.Host, FunctionKey.Memo)]
        public ActionResult Create(Memo model)
        {
            var getAllMemoCriteria = new GetAllLookupDataCriteria { LookupCode = Constants.MemoCategory };

            ViewBag.MemoCategories = lookupDataService.GetAll(getAllMemoCriteria);
            model.Contents = model.MemoType == UploadMemoType ? model.AttachFile : model.Contents;

            if (ModelState.IsValid)
            {
                var insertMemoCriteria = new InsertMemoCriteria
                {
                    RefNo = model.RefNo,
                    Contents = model.Contents,
                    Subject = model.Subject,
                    SentTo = model.SentTo,
                    SentFrom = model.SentFrom,
                    SentDate = model.SentDate,
                    ForInfo = model.ForInfo,
                    TotalPages = model.TotalPages,
                    Category = model.Category,
                    MemoType = model.MemoType,
                    SendMail = model.SendMail
                };

                CommandResult result = commandService.Insert(insertMemoCriteria);

                ViewBag.Updated = result.IsSuccessful;

                if (result.IsSuccessful && insertMemoCriteria.SendMail)
                {
                    SendMail(insertMemoCriteria.Subject, result);
                }

                if (!result.IsSuccessful)
                {
                    ModelState.AddModelError(string.Empty, result.Message);
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, Resources.Messages.InvalidInputData);
            }

            return View(model);
        }

        [HttpPost]
        [HRAuthorize(Policy.Host, FunctionKey.Memo)]
        public JsonResult DeleteMemo(int id)
        {
            var criteria = new DeleteMemoCriteria { Id = id };
            Memo memo = memoService.GetById(new GetMemoByIdCriteria { Id = id });
            CommandResult result = commandService.Delete(criteria);

            if (result.IsSuccessful && memo.MemoType == UploadMemoType)
            {
                var allFiles = memo.AttachFile.Split(';');

                foreach (var fileName in allFiles)
                {
                    System.IO.File.Delete(Server.MapPath("~/" + fileName));
                }
            }

            return Json(result);
        }

        [HRAuthorize(Policy.Read, FunctionKey.Memo)]
        public ActionResult Detail(int id)
        {
            var criteria = new GetMemoByIdCriteria { Id = id };
            Memo memo = memoService.GetById(criteria);

            return View(memo);
        }

        private void SendMail(string subject, CommandResult result)
        {
            var link = Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath;
            var groupSd = ConfigurationManager.AppSettings["EmailSD"];
            var groupGo = ConfigurationManager.AppSettings["EmailGo"];

            var bodyTemplate = Utilities.ReadTextFile(Server.MapPath("~/app_data/emails/MemoNotification.cshtml"));
            var body = string.Format(bodyTemplate, subject, link + "memo/detail/" + result);

            emailService.SendEmail(
                ConfigurationManager.AppSettings["Email"],
                groupGo,
                string.Empty,
                Resources.Messages.MemoSubject + " " + subject,
                body);
            emailService.SendEmail(
               ConfigurationManager.AppSettings["Email"],
               groupSd,
               string.Empty,
               Resources.Messages.MemoSubject + " " + subject,
               body);
        }
    }
}