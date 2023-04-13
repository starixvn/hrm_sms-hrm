namespace GO.Website.Controllers.API
{
    using System.Threading.Tasks;
    using GO.Website.Hubs;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.SignalR;

    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class TodolistController : ControllerBase
    {
        private readonly IHubContext<TodolistHub> hubContext;

        public TodolistController(IHubContext<TodolistHub> hubContext)
        {
            this.hubContext = hubContext;
        }

        [HttpPost]
        public async Task<IActionResult> NotiRefreshHrm()
        {
            await hubContext.Clients.All.SendAsync("RefreshHrm");
            return Ok();
        }
    }
}