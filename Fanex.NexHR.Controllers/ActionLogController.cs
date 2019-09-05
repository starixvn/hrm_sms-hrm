namespace Fanex.NexHR.Controllers
{
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Fanex.NexHR.Core.Services;
    using Fanex.NexHR.Models.Log;
    using Fanex.NexHR.Services;

    public class ActionLogController : BaseController
    {
        private readonly IActionLogService actionLogService;

        public ActionLogController(
            ISessionService sessionService,
            IActionLogService actionLogService) : base(sessionService)
        {
            this.actionLogService = actionLogService;
        }

        public ActionResult Details(int id, byte objectTypeId)
        {
            IEnumerable<LogObjectDetail> logs = actionLogService.GetLogDetailById(objectTypeId, id);

            return View("Details", logs);
        }
    }
}