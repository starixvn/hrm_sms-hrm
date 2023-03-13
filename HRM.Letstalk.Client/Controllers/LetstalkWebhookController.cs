namespace HRM.Letstalk.Client.Controllers
{
    using HRM.Letstalk.Client.Models;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using HRM.Letstalk.Client.Services.Interface;
    using System.Collections.Generic;
    using System;
    using System.Linq;

    public class LetstalkWebhookController : ControllerBase
    {
        private readonly Func<int, ILetstalkService> letstalkService;
        private readonly List<WebhookConfiguration> webhookConfigurations;

        public LetstalkWebhookController(
            Func<int, ILetstalkService> letstalkService,
            List<WebhookConfiguration> webhookConfigurations
            )
        {
            this.letstalkService = letstalkService;
            this.webhookConfigurations = webhookConfigurations;
        }

        [Route("api/setwebhook/{id}")]
        public async Task<IActionResult> SetWebhook(int id)
        {
            var botConfigs = webhookConfigurations.FirstOrDefault(config => config.BotId == id);
            if (botConfigs != null && botConfigs.IsAllowedSet)
            {
                var webhookUrl = $"{botConfigs.WebhookBaseUrl}api/webhook/{id}";
                var result = await letstalkService(id).SetWebhookAsync(webhookUrl);
                return Ok(result);
            }

            return NotFound();
        }

        [Route("api/getwebhook/{id}")]
        public async Task<IActionResult> GetWebhook(int id)
        {
            var botConfigs = webhookConfigurations.FirstOrDefault(config => config.BotId == id);
            if (botConfigs != null && botConfigs.IsAllowedGet)
            {
                var result = await letstalkService(id).GetWebhookAsync();
                return Ok(result);
            }

            return NotFound();
        }
    }
}
