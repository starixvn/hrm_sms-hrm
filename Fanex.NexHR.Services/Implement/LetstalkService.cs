namespace Fanex.NexHR.Services.Implement
{
    using System.Collections.Generic;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Services.Interfaces;
    using Hangfire;
    using HRM.Letstalk.Core.Interface;

    public class LetstalkService : ILetstalkService
    {
        private readonly ISendNotificationService sendNotificationService;

        public LetstalkService(ISendNotificationService sendNotificationService)
        {
            this.sendNotificationService = sendNotificationService;
        }

        public void SendNotification(string letstalkId, string message)
        {
            var botId = ConfigSettings.BotId;
            var url = ConfigSettings.UrlSendLetstalk;
            var username = ConfigSettings.AppCredentialId;
            var pass = ConfigSettings.AppCredentialPassword;

            var jwtToken = sendNotificationService.GetToken(url, username, pass);
            if (jwtToken != null)
            {
                var data = new Dictionary<string, object>(){
                    { "BotId", botId },
                    { "ConversationId", letstalkId },
                    { "Message", message }
                };
                BackgroundJob.Enqueue(() =>
                       sendNotificationService.SendNotificationToLetstalk1(url, jwtToken.Token, data)
                );
            }
        }
    }
}
