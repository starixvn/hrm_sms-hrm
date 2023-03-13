namespace HRM.Letstalk.Client.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using HRM.Letstalk.Client.Models;
    using HRM.Letstalk.Client.Models.Result;
    using HRM.Letstalk.Client.Services.Interface;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;

    public class LetstalkService : ILetstalkService
    {
        private const int payloadId = 100;
        private readonly BotConfig botConfig;
        private readonly IHttpClientService httpClientService;
        private readonly ICryptographyService cryptographyService;

        private readonly JsonSerializerSettings DefaultJsonSerializeSetting = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };

        public LetstalkService(ICryptographyService cryptographyService,
            IHttpClientService httpClientService,
            BotConfig botConfig)
        {
            this.cryptographyService = cryptographyService;
            this.httpClientService = httpClientService;
            this.botConfig = botConfig;
        }

        public async Task<PushMessageResult> PushAsync(IEnumerable<string> accountIds, Payload payload)
        {
            ValidateInputParam(nameof(accountIds), accountIds);
            ValidateInputParam(nameof(payload), payload);

            var accountIdToLowerList = accountIds.Select(accountId => accountId.ToLowerInvariant());

            payload.UserId = botConfig.BotUserId;
            payload.Time = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            payload.HashType = botConfig.EncryptionMode;
            payload.From = botConfig.BusinessAccountType;
            payload.Type = MessageType.Text;
            payload.Id = payloadId;
            payload.Receiver = accountIdToLowerList.ElementAt(0);
            payload.HashId = botConfig.ChatKeyVersion;

            var result = await httpClientService.ExecutePostAsync<PushMessageResult>(
                $"bot/push/{botConfig.BotAccountId}",
                new Dictionary<string, object>
                {
                    { "receiver", accountIdToLowerList },
                    { "payload", JsonConvert.SerializeObject(payload, DefaultJsonSerializeSetting) }
                });

            return result;
        }

        public async Task<PushMessageResult> PushAsync(IEnumerable<string> accountIds, string message, DateTime? expiredTime = null)
                => await PushAsync(accountIds, new Message { Content = message }, expiredTime);

        public async Task<PushMessageResult> PushAsync(string accountId, string message, DateTime? expiredTime = null)
                => await PushAsync(new List<string> { accountId }, message, expiredTime);

        public async Task<PushMessageResult> PushAsync(IEnumerable<string> accountIds, Message message, DateTime? expiredTime = null)
        {
            ValidateInputParam(nameof(accountIds), accountIds);
            ValidateInputParam(nameof(message), message);

            var payload = new Payload();
            var formattedMessage = JsonConvert.SerializeObject(message, DefaultJsonSerializeSetting);
            payload.HashContentKey = cryptographyService.Hash(formattedMessage);
            payload.HashContent = cryptographyService.Encrypt(formattedMessage, botConfig.ChatKey, payload.HashContentKey);

            return await PushAsync(accountIds, payload);
        }

        public async Task<BaseResult> PushRoomAsync(string chatroomToken, string message, DateTime? expiredTime = null)
        {
            ValidateInputParam(nameof(chatroomToken), chatroomToken);
            ValidateInputParam(nameof(message), message);

            var result = await httpClientService.ExecutePostAsync<BaseResult>(
               $"bot/push_chatroom/{botConfig.BotAccountId}",
               new Dictionary<string, object>
               {
                    { "receiver", chatroomToken },
                    { "msg", message }
               });

            return result ?? new BaseResult();
        }

        public async Task<string> GetWebhookAsync()
        {
            var result = await httpClientService.ExecuteGetAsync<WebhookResult>($"bot/get_webhook/{botConfig.BotAccountId}");

            if (result.IsSuccess)
            {
                return result.Url;
            }

            return string.Empty;
        }

        public async Task<BaseResult> SetWebhookAsync(string url)
        {
            ValidateInputParam(nameof(url), url);
            return await httpClientService.ExecutePostAsync<BaseResult>(
                $"bot/set_webhook/{botConfig.BotAccountId}",
                new Dictionary<string, object>
                {
                    { "url", url }
                });
        }


        private static void ValidateInputParam(string paramName, object paramValue)
        {
            if (paramValue == null
                || (paramValue is string && string.IsNullOrWhiteSpace(paramValue as string)))
            {
                throw new ArgumentNullException(paramName);
            }
        }
    }
}
