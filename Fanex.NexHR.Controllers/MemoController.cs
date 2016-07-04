namespace Fanex.NexHR.Controllers
{
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Criteria.Lookup;
    using Fanex.NexHR.Criteria.Memo;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Repositories;
    using Fanex.NexHR.Services.Interfaces;
    using Fanex.NexHR.ViewModels.Memo;
    using Fanex.NexHR.Web.Providers;
    using Nexus.RoleManagement.Services;
    using System;
    using System.Web.Mvc;

    public class MemoController : BaseController
    {
        private readonly IMemoService memoService;
        private readonly ILookupDataService lookupDataService;

        public MemoController(IMemoService memoService = null, ILookupDataService lookupDataService = null)
        {
            this.memoService = memoService ?? new MemoService();
            this.lookupDataService = lookupDataService ?? new LookupDataService();
        }

        public MemoController()
            : this(null)
        {
        }

        [HRAuthorize(Policy.Read, FunctionKey.Memo)]
        public ActionResult Index()
        {
            var getAllMemoCriteria = new GetAllLookupDataCriteria
            {
                LookupCode = Constants.MemoCategory
            };
            ViewBag.MemoCategories = lookupDataService.GetAll(getAllMemoCriteria);
            MemoViewModel model = new MemoViewModel
            {
                Content = string.Empty,
                Year = DateTime.Now.Year,
                Category = -1
            };

            ViewBag.HasPermission = CurrentUser.HasPermission(FunctionKey.Memo, Policy.Host);

            return Index(model);
        }

        [HttpPost]
        [HRAuthorize(Policy.Read, FunctionKey.Memo)]
        public ActionResult Index(MemoViewModel model)
        {
            var getAllMemoCriteria = new GetAllLookupDataCriteria
            {
                LookupCode = Constants.MemoCategory
            };
            ViewBag.MemoCategories = lookupDataService.GetAll(getAllMemoCriteria);
            var criteria = new GetAllMemoCriteria
            {
                Content = model.Content,
                Year = model.Year,
                Category = model.Category
            };
            ViewBag.EntityList = memoService.GetAll(criteria);
            ViewBag.HasPermission = CurrentUser.HasPermission(FunctionKey.Memo, Policy.Host);

            return View(model);
        }

        [HttpGet]
        [HRAuthorize(Policy.Host, FunctionKey.Memo)]
        public ActionResult Edit(int id)
        {
            var getAllMemoCriteria = new GetAllLookupDataCriteria
            {
                LookupCode = Constants.MemoCategory
            };
            ViewBag.MemoCategories = lookupDataService.GetAll(getAllMemoCriteria);
            var criteria = new GetMemoByIdCriteria
            {
                Id = id
            };
            var memo = memoService.GetById(criteria);
            ViewBag.Updated = false;

            return View(memo);
        }

        /// [Authorize(Roles = "HR")]
        [HttpPost]
        [ValidateInput(false)]
        [HRAuthorize(Policy.Host, FunctionKey.Memo)]
        public ActionResult Edit(Memo model)
        {
            var getAllMemoCriteria = new GetAllLookupDataCriteria
            {
                LookupCode = Constants.MemoCategory
            };
            ViewBag.MemoCategories = lookupDataService.GetAll(getAllMemoCriteria);
            if (model.MemoType == 1 && string.IsNullOrEmpty(model.AttachFile))
            {
                return View(model);
            }

            if (model.MemoType == 1)
            {
                model.Contents = model.AttachFile;
            }

            if (ModelState.IsValid)
            {
                var criteria = new UpdateMemoCriteria
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
                CommandResult result = null;
                result = memoService.Update(criteria);
                ViewBag.Updated = result.IsSuccessful;
                if (!result.IsSuccessful)
                {
                    ModelState.AddModelError(string.Empty, "Edit memo failed!");
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, Resources.Messages.InvalidInputData);
            }

            return View(model);
        }

        /// [Authorize(Roles = "HR")]
        [HRAuthorize(Policy.Host, FunctionKey.Memo)]
        public ActionResult Create()
        {
            var getAllMemoCriteria = new GetAllLookupDataCriteria
            {
                LookupCode = Constants.MemoCategory
            };
            ViewBag.MemoCategories = lookupDataService.GetAll(getAllMemoCriteria);
            ViewBag.Updated = false;

            return View(new Memo
            {
                SentTo = "All Staff",
                SentFrom = "Management",
                TotalPages = 1,
                SentDate = DateTime.Now,
                Contents = "[Contents]",
                MemoType = 1
            });
        }

        /// [Authorize(Roles = "HR")]
        [HttpPost]
        [ValidateInput(false)]
        [HRAuthorize(Policy.Host, FunctionKey.Memo)]
        public ActionResult Create(Memo model)
        {
            var getAllMemoCriteria = new GetAllLookupDataCriteria
            {
                LookupCode = Constants.MemoCategory
            };
            ViewBag.MemoCategories = lookupDataService.GetAll(getAllMemoCriteria);
            if (model.MemoType == 1)
            {
                model.Contents = model.AttachFile;
            }

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
                CommandResult result = memoService.Insert(insertMemoCriteria);

                ViewBag.Updated = result.IsSuccessful;

                if (!result.IsSuccessful)
                {
                    ModelState.AddModelError(string.Empty, "Duplicate memo");
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, Resources.Messages.InvalidInputData);
            }

            return View(model);
        }

        /// [Authorize(Roles = "HR")]
        [HttpPost]
        [HRAuthorize(Policy.Host, FunctionKey.Memo)]
        public JsonResult DeleteMemo(int id)
        {
            var criteria = new DeleteMemoCriteria
            {
                Id = id
            };

            CommandResult result = null;
            result = memoService.Delete(criteria);
            return Json(result);
        }

        [HRAuthorize(Policy.Read, FunctionKey.Memo)]
        public ActionResult Detail(int id)
        {
            var criteria = new GetMemoByIdCriteria
            {
                Id = id
            };
            return View(memoService.GetById(criteria));
        }
    }
}