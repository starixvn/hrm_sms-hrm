namespace Fanex.NexHR.Controllers
{
    using Resources;
    using Services;
    using Services.Interfaces;
    using System.Configuration;
    using System.Web.Mvc;

    public class EmailController : BaseController
    {
        private readonly IEmailService emailService;

        public EmailController()
            : this(null)
        {
        }

        public EmailController(IEmailService emailService)
        {
            this.emailService = emailService ?? new EmailService();
        }

        [HttpGet]
        public JsonResult Index()
        {
            emailService.SendEmail(ConfigurationManager.AppSettings["EmailSD"], ConfigurationManager.AppSettings["EmailSD"], string.Empty, Labels.TestSendMail, Labels.TestSendMail);
            return Json("Done", JsonRequestBehavior.AllowGet);
        }
    }
}