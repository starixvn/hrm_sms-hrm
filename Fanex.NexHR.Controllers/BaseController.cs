namespace Fanex.NexHR.Controllers
{
    using System.Web.Mvc;
    using Fanex.NexHR.Core.Filters;
    using Fanex.NexHR.Core.Services;

    [HRMAuthorize]
    public class BaseController : Controller
    {
        protected readonly ISessionService sessionService;

        public BaseController(ISessionService sessionService)
        {
            this.sessionService = sessionService;
        }
    }
}