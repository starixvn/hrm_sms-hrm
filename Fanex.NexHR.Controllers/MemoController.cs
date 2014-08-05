// --------------------------------------------------------------------------
// <copyright file="MemoController.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.15">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.Controllers
{
    using System;
    using System.Web.Mvc;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Criteria.Lookup;
    using Fanex.NexHR.Criteria.Memo;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Repositories;
    using Fanex.NexHR.Services.Interfaces;
    using Fanex.NexHR.ViewModels;

    /// <summary>
    /// Memo Controller.
    /// </summary>
    [Authorize]
    public class MemoController : Controller
    {
        private readonly IMemoService _memoService;
        private readonly ILookupDataService _lookupDataService;

        public MemoController(IMemoService memoService = null, ILookupDataService lookupDataService = null)
        {
            _memoService = memoService ?? new MemoService();
            _lookupDataService = lookupDataService ?? new LookupDataService();
        }

        public MemoController()
            : this(null)
        { 
        }

        /// <summary>
        /// ActionResult Index.
        /// </summary>
        /// <returns>View Index.</returns>
        public ActionResult Index()
        {
            var getAllMemoCriteria = new GetAllLookupDataCriteria
            {
                LookupCode = Constants.MemoCategory
            };
            ViewBag.MemoCategories = _lookupDataService.GetAll(getAllMemoCriteria);
            MemoViewModel model = new MemoViewModel
            {
                Content = string.Empty,
                Year = DateTime.Now.Year,
                Category = -1
            };

            return Index(model);
        }

        /// <summary>
        /// Get all memo.
        /// </summary>
        /// <param name="model">The Memo model.</param>
        /// <returns>The view index.</returns>
        [HttpPost]
        public ActionResult Index(MemoViewModel model)
        {
            var getAllMemoCriteria = new GetAllLookupDataCriteria
            {
                LookupCode = Constants.MemoCategory
            };
            ViewBag.MemoCategories = _lookupDataService.GetAll(getAllMemoCriteria);
            var criteria = new GetAllMemoCriteria
            {
                Content = model.Content,
                Year = model.Year,
                Category = model.Category
            };
            ViewBag.EntityList = _memoService.GetAll(criteria);
            return View(model);
        }

        /// <summary>
        /// Edit memo.
        /// </summary>
        /// <param name="id">The memo Id.</param>
        /// <returns>The Action Result.</returns>
        [HttpGet]
        public ActionResult Edit(int id)
        {
            var getAllMemoCriteria = new GetAllLookupDataCriteria
            {
                LookupCode = Constants.MemoCategory
            };
            ViewBag.MemoCategories = _lookupDataService.GetAll(getAllMemoCriteria);
            var criteria = new GetMemoByIdCriteria
            {
                Id = id
            };
            var memo = _memoService.GetById(criteria);
            return View(memo);
        }

        /// <summary>
        /// ActionResult Edit.
        /// </summary>
        /// <param name="model">Memo model.</param>
        /// <returns>View Edit.</returns>
        [Authorize(Roles = "HR")]
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(Memo model)
        {
            var getAllMemoCriteria = new GetAllLookupDataCriteria
            {
                LookupCode = Constants.MemoCategory
            };
            ViewBag.MemoCategories = _lookupDataService.GetAll(getAllMemoCriteria);
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
                ViewBag.Updated = _memoService.Update(criteria);
            }
            else
            {
                ModelState.AddModelError(string.Empty, Resources.Messages.InvalidInputData);
            }

            return View(model);
        }

        /// <summary>
        /// ActionResult Create.
        /// </summary>
        /// <returns>View Create.</returns>
        [Authorize(Roles = "HR")]
        public ActionResult Create()
        {
            var getAllMemoCriteria = new GetAllLookupDataCriteria
            {
                LookupCode = Constants.MemoCategory
            };
            ViewBag.MemoCategories = _lookupDataService.GetAll(getAllMemoCriteria);
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

        /// <summary>
        /// ActionResult Create.
        /// </summary>
        /// <param name="model">Memo model.</param>
        /// <returns>View Create.</returns>
        [Authorize(Roles = "HR")]
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(Memo model)
        {
            var getAllMemoCriteria = new GetAllLookupDataCriteria
            {
                LookupCode = Constants.MemoCategory
            };
            ViewBag.MemoCategories = _lookupDataService.GetAll(getAllMemoCriteria);
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
                int memoId = _memoService.Insert(insertMemoCriteria);

                ViewBag.Updated = memoId != -1 ? true : false;

                if (memoId == -1)
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

        public ActionResult Detail(int id)
        {
            var criteria = new GetMemoByIdCriteria
            {
                Id = id
            };
            return View(_memoService.GetById(criteria));
        }
    }
}