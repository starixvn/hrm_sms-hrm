// --------------------------------------------------------------------------
// <copyright file="MemoService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.15">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.Repositories
{
    using System.Collections.Generic;
    using System.Configuration;
    using System.Web;
    using Fanex.Common.Helper;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Criteria.Memo;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Repositories.Interfaces;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interfaces;

    /// <summary>
    /// Memo Service.
    /// </summary>
    public class MemoService : BaseService, IMemoService
    {
        private readonly IMemoRepository _memoRepository;
        private readonly IEmailService _emailService;

        public MemoService()
            : this(null, null)
        {
        }

        public MemoService(IMemoRepository memoRepository, IEmailService emailService)
        {
            this._memoRepository = memoRepository ?? new MemoRepository();
            this._emailService = emailService ?? new EmailService();
        }

        /// <summary>
        /// Get all Memo.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The enumerator Memo.</returns>
        public IEnumerable<Memo> GetAll(GetAllMemoCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IEnumerable<Memo> entityList = null;

            if (criteria.IsValid())
            {
                entityList = GetData<Memo>(criteria);
            }

            return entityList;
        }

        /// <summary>
        /// Get memo by id.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>Memo object.</returns>
        public Memo GetById(GetMemoByIdCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            Memo memo = new Memo();

            if (criteria.IsValid())
            {
                memo = GetDataEntity<Memo>(criteria);

                if (memo != null && memo.MemoType == 1)
                {
                    memo.AttachFile = memo.Contents;
                }
            }

            return memo;
        }

        /// <summary>
        /// Delete Memo.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The result.</returns>
        public bool Delete(DeleteMemoCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            bool result = false;

            if (criteria.IsValid())
            {
                result = _memoRepository.Delete(criteria);
            }

            return result;
        }

        /// <summary>
        /// Function to insert MEMO.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>Result of insert.</returns>
        public int Insert(InsertMemoCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            int result = 0;

            if (criteria.IsValid())
            {
                result = this._memoRepository.Insert(criteria);

                if (result != -1 && criteria.SendMail)
                {
                    var link = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority
                            + HttpContext.Current.Request.ApplicationPath;

                    var groupSd = ConfigurationManager.AppSettings["EmailSD"].ToString();
                    var groupGo = ConfigurationManager.AppSettings["EmailGo"].ToString();
                    //// send email(s)
                    var bodyTemplate = Utilities.ReadTextFile(HttpContext.Current.Server.MapPath("~/app_data/emails/MemoNotification.cshtml"));

                    var content = new
                    {
                        Subject = criteria.Subject,
                        Link = link + "memo/detail/" + result
                    };
                    var body = RazorEngine.Razor.Parse(bodyTemplate, content);
                    this._emailService.SendEmail(
                        ConfigurationManager.AppSettings["Email"],
                        groupGo,
                        string.Empty,
                        Resources.Messages.MemoSubject + " " + criteria.Subject,
                        body);
                    this._emailService.SendEmail(
                       ConfigurationManager.AppSettings["Email"],
                       groupSd,
                       string.Empty,
                       Resources.Messages.MemoSubject + " " + criteria.Subject,
                       body);
                }
            }

            return result;
        }

        /// <summary>
        /// Function to update MEMO.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>Result of updated.</returns>
        public bool Update(UpdateMemoCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            bool result = false;

            if (criteria.IsValid())
            {
                result = this._memoRepository.Update(criteria);

                if (result && criteria.SendMail)
                {
                    var link = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority 
                                    + HttpContext.Current.Request.ApplicationPath;

                    var groupSd = ConfigurationManager.AppSettings["EmailSD"].ToString();
                    var groupGo = ConfigurationManager.AppSettings["EmailGo"].ToString();
                    //// send email(s)
                    var bodyTemplate = Utilities.ReadTextFile(HttpContext.Current.Server.MapPath("~/app_data/emails/EditMemoNotification.cshtml"));

                    var content = new
                    {
                        Subject = criteria.Subject,
                        Link = link + "memo/detail/" + criteria.Id
                    };
                    var body = RazorEngine.Razor.Parse(bodyTemplate, content);
                    this._emailService.SendEmail(
                        ConfigurationManager.AppSettings["Email"],
                        groupGo,
                        string.Empty,
                        Resources.Messages.MemoSubject + " " + criteria.Subject,
                        body);
                    this._emailService.SendEmail(
                       ConfigurationManager.AppSettings["Email"],
                       groupSd,
                       string.Empty,
                       Resources.Messages.MemoSubject + " " + criteria.Subject,
                       body);
                }
            }

            return result;
        }
    }
}
