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
            emailService.SendEmail(ConfigurationManager.AppSettings["EmailSD"], ConfigurationManager.AppSettings["EmailSD"], string.Empty, Labels.TestSendMail, Labels.TestSendMail);
            return Json("Done", JsonRequestBehavior.AllowGet);
        }
    }
}