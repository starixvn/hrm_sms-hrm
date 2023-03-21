namespace Fanex.NexHR.Services.Implement
{
    using System;
    using System.Collections.Generic;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Services.Interfaces;
    using Hangfire;
    using HRM.Letstalk.Core.Interface;

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
            if (!string.IsNullOrEmpty(letstalkId))
            {
                var botId = ConfigSettings.BotId;
                var url = ConfigSettings.UrlSendLetstalk;
                var username = ConfigSettings.AppCredentialId;
                var pass = ConfigSettings.AppCredentialPassword;

                var content = string.Format(templateContent, subject, message);

                var jwtToken = sendNotificationService.GetToken(url, username, pass);
                if (jwtToken != null)
                {
                    var data = new Dictionary<string, object>(){
                    { "BotId", botId },
                    { "ConversationId", letstalkId },
                    { "Message", content }
                };
                    BackgroundJob.Enqueue(() =>
                           sendNotificationService.SendNotificationToLetstalk1(url, jwtToken.Token, data)
                    );
                }
            }
        }
    }
}
