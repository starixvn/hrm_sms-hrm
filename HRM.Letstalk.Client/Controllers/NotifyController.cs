namespace HRM.Letstalk.Client.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using System.Threading.Tasks;
    using System;
    using Microsoft.AspNetCore.Mvc;
    using static HRM.Letstalk.Client.Services.LogService;
    using HRM.Letstalk.Client.Services.Interface;
    using HRM.Letstalk.Client.Models;
    using System.Collections.Generic;
    using System.Linq;
    using HRM.Letstalk.Client.Models.Result;
    using HRM.Letstalk.Client.Constants;
    using Newtonsoft.Json;
    using HRM.Letstalk.Client.Enum;

    [Authorize]
    public class NotifyController : ControllerBase
    {
        private readonly ILetstalkLogService logService;
        private readonly Func<int, ILetstalkService> letstalkService;
        private readonly List<WebhookConfiguration> webhookConfigurations;

        public NotifyController(ILetstalkLogService logService,
            List<WebhookConfiguration> webhookConfigurations,
            Func<int, ILetstalkService> letstalkService
            )
        {
            this.logService = logService;
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
                logService.LogError(ex);
            }

            return Ok(errorMessage);
        }

        private async Task SendToLestalk(int botId, string conversationId, string message)
        {
            var sendResult = new SendNotificationResult
            {
                ConversationId = conversationId
            };

            var outGoingMsg = new OutgoingMessage
            {
                MessageType = MessageTypeNumber.Html,
                Text = message
            };

            var botConfigs = webhookConfigurations.FirstOrDefault(config => config.BotId == botId);
            if (botConfigs != null && botConfigs.IsAllowedSet)
            {
                PushMessageResult result = await letstalkService(botConfigs.BotId).PushMulticastAsync(new List<string> { conversationId }, outGoingMsg);
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

                logService.LogInformation(JsonConvert.SerializeObject(sendResult, Formatting.Indented));
            }
        }
    }
}
