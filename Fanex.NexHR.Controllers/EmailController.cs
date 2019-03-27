namespace Fanex.NexHR.Controllers
{
    using System.Configuration;
    using System.Web.Mvc;
    using Resources;
    using Services;

    public class EmailController : BaseController
    {
        private readonly IEmailService emailService;

        public EmailController(IEmailService emailService)
        {
            this.emailService = emailService;
        }

        [HttpGet]
        public JsonResult Index()
        {
            string emailSD = ConfigurationManager.AppSettings["EmailSD"];
            emailService.SendEmail(
                emailSD,
                receiverEmail: emailSD,
                mailCC: string.Empty, subject: Labels.TestSendMail,
                body: Labels.TestSendMail);

            return Json("Done", JsonRequestBehavior.AllowGet);
        }
    }
}