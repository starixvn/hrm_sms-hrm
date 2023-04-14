namespace GO.Letstalk.WebAPI.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using GO.Letstalk.WebAPI.Constants;
    using GO.Letstalk.WebAPI.Enums;
    using GO.Letstalk.WebAPI.Models;
    using GO.Letstalk.WebAPI.Models.Result;
    using GO.Letstalk.WebAPI.Services;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;

    [Authorize]
    public class NotifyController : ControllerBase
    {
        private readonly ILogger logger;
        private readonly Func<int, ILetstalkService> letstalkService;
        private readonly List<WebhookConfiguration> webhookConfigurations;

        public NotifyController(
            ILogger logger,
            List<WebhookConfiguration> webhookConfigurations,
            Func<int, ILetstalkService> letstalkService
            )
        {
            this.logger = logger;
            this.webhookConfigurations = webhookConfigurations;
            this.letstalkService = letstalkService;
        }

        [HttpPost]
        [Route("api/notify")]
        public async Task<ObjectResult> Send([FromBody] NotifyModel model)
        {
            var errorMessage = string.Empty;
            try
            {
                await SendToLestalk(model.BotId, model.ConversationId, model.Message);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                logger.LogError(ex, ex.Message);
            }

            return Ok(errorMessage);
        }

        private async Task SendToLestalk(int botId, string conversationId, string message)
        {
            var sendResult = new SendNotificationResult
            {
                ConversationId = string.Join(", ", conversationId)
            };

            var outGoingMsg = new OutgoingMessage
            {
                MessageType = MessageTypeNumber.Html,
                Text = message
            };

            var conversationIds = conversationId.Contains(";") ? conversationId.Split(";").ToList() : new List<string> { conversationId };

            var botConfigs = webhookConfigurations.FirstOrDefault(config => config.BotId == botId);
            if (botConfigs != null && botConfigs.IsAllowedSet)
            {
                PushMessageResult result = await letstalkService(botConfigs.BotId).PushMulticastAsync(conversationIds, outGoingMsg);
                if (result.IsSuccess && !result.BlockList.Any())
                {
                    sendResult.Status = SendNotificationStatus.Sent;
                    sendResult.ErrMsg = string.Empty;
                }

                if (result.FailList.Any())
                {
                    sendResult.Status = SendNotificationStatus.Error;
                    sendResult.ErrMsg = SendNotificationErrMsg.LetstalkIdInvalidMsg;
                }

                if (result.BlockList.Any())
                {
                    sendResult.Status = SendNotificationStatus.BotBlocked;
                    sendResult.ErrMsg = string.Empty;
                }

                logger.LogInformation(JsonConvert.SerializeObject(sendResult, Formatting.Indented));
            }
        }
    }
}