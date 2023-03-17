namespace Fanex.NexHR.Controllers
{
    using Fanex.NexHR.Services.Interfaces;
    using System.Web.Mvc;
    public class SendController : Controller
    {
        private readonly ILetstalkService letstalkService;

        public SendController(ILetstalkService letstalkService)
        {
            this.letstalkService = letstalkService;
        }

        public void sendNoti()
        {
            letstalkService.SendNotification("t02031222", "test send letstalk service 123");
        }
    }
}
