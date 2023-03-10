using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Mvc;
using static HRM.Letstalk.Client.Services.LogService;

namespace HRM.Letstalk.Client.Controllers
{
    [Authorize]
    public class NotifyController : Controller
    {
        private readonly ILetstalkLogService logService;
        public NotifyController(ILetstalkLogService logService)
        {
            this.logService = logService;
        }

        //[AllowAnonymous]
        [HttpGet]
        [Route("api/Test")]
        public async Task<ObjectResult> Test()
        {
            logService.LogInformation("test sentry");

            return Ok("ok");
        }
    }
}
