namespace Fanex.NexHR.Repositories
{
    using Fanex.Common.Helper;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Criteria.Memo;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Repositories.Interfaces;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interfaces;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Web;

    public class MemoService : BaseService, IMemoService
    {
        private readonly ICommandService commandService;
        private readonly IEmailService emailService;

        public MemoService()
            : this(null, null)
        {
        }

        public MemoService(ICommandService commandService, IEmailService emailService)
        {
            this.commandService = commandService ?? new CommandService();
            this.emailService = emailService ?? new EmailService();
        }

        public IEnumerable<Memo> GetAll(GetAllMemoCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IEnumerable<Memo> entityList = null;

            entityList = GetData<Memo>(criteria);

            return entityList;
        }

        public Memo GetById(GetMemoByIdCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            Memo memo = new Memo();
            memo = GetDataEntity<Memo>(criteria);
            if (memo != null && memo.MemoType == 1)
            {
                memo.AttachFile = memo.Contents;
            }

            return memo;
        }

        public CommandResult Delete(DeleteMemoCriteria criteria)
        {
            CommandResult result = null;
            Check.Argument.IsNotNull(criteria, "criteria");

            if (criteria.IsValid())
            {
                result = commandService.Delete(criteria);
            }

            return result;
        }

        public CommandResult Insert(InsertMemoCriteria criteria)
        {
            CommandResult result = null;
            Check.Argument.IsNotNull(criteria, "criteria");

            if (criteria.IsValid())
            {
                result = this.commandService.Insert(criteria);

                if (result.IsSuccessful && criteria.SendMail)
                {
                    var link = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority
                            + HttpContext.Current.Request.ApplicationPath;

                    var groupSd = ConfigurationManager.AppSettings["EmailSD"].ToString();
                    var groupGo = ConfigurationManager.AppSettings["EmailGo"].ToString();
                    //// send email(s)
                    var bodyTemplate = Utilities.ReadTextFile(HttpContext.Current.Server.MapPath("~/app_data/emails/MemoNotification.cshtml"));
                    var body = string.Format(bodyTemplate, criteria.Subject, link + "memo/detail/" + result);
                    this.emailService.SendEmail(
                        ConfigurationManager.AppSettings["Email"],
                        groupGo,
                        string.Empty,
                        Resources.Messages.MemoSubject + " " + criteria.Subject,
                        body);
                    this.emailService.SendEmail(
                       ConfigurationManager.AppSettings["Email"],
                       groupSd,
                       string.Empty,
                       Resources.Messages.MemoSubject + " " + criteria.Subject,
                       body);
                }
            }

            return result;
        }

        public CommandResult Update(UpdateMemoCriteria criteria)
        {
            CommandResult result = null;
            Check.Argument.IsNotNull(criteria, "criteria");

            if (criteria.IsValid())
            {
                result = this.commandService.Update(criteria);

                if (result.IsSuccessful && criteria.SendMail)
                {
                    var link = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority
                                    + HttpContext.Current.Request.ApplicationPath;

                    var groupSd = ConfigurationManager.AppSettings["EmailSD"].ToString();
                    var groupGo = ConfigurationManager.AppSettings["EmailGo"].ToString();
                    //// send email(s)
                    var bodyTemplate = Utilities.ReadTextFile(HttpContext.Current.Server.MapPath("~/app_data/emails/EditMemoNotification.cshtml"));
                    var body = string.Format(bodyTemplate, criteria.Subject, link + "memo/detail/" + result);
                    this.emailService.SendEmail(
                        ConfigurationManager.AppSettings["Email"],
                        groupGo,
                        string.Empty,
                        Resources.Messages.MemoSubject + " " + criteria.Subject,
                        body);
                    this.emailService.SendEmail(
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
