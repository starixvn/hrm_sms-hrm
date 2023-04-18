namespace Fanex.NexHR.Services.Implement
{
    using System.Collections.Generic;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Services.Interfaces;
    using HRM.Letstalk.Core.Services;

    public class LetstalkService : ILetstalkService
    {
        private readonly ISendNotificationService sendNotificationService;
        private readonly string templateContent = "<strong>{0}</strong><br /> {1}";

        public LetstalkService(ISendNotificationService sendNotificationService)
        {
            this.sendNotificationService = sendNotificationService;
        }

        public void SendNotification(string letstalkId, string subject, string message)
        {
            if (ConfigSettings.IsEnableSendLetstalk && !string.IsNullOrWhiteSpace(letstalkId))
            {
                var jwtToken = sendNotificationService.GetToken(ConfigSettings.Letstalk_API_BaseUrl,
                    ConfigSettings.Letstalk_Token_Username,
                    ConfigSettings.Letstalk_Token_Password);
                if (jwtToken != null)
                {
                    var content = string.Format(templateContent, subject, message);
                    var data = new Dictionary<string, object>
                    {
                        { "BotId", ConfigSettings.Letstalk_BotId },
                        { "ConversationId", letstalkId },
                        { "Message", content }
                    };

                    sendNotificationService.SendNotificationToLetstalk(ConfigSettings.Letstalk_API_BaseUrl, jwtToken.Token, data);
                }
            }
        }
    }
}