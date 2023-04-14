namespace GO.Letstalk.WebAPI.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using GO.Letstalk.WebAPI.Enums;
    using GO.Letstalk.WebAPI.Models;
    using GO.Letstalk.WebAPI.Models.Result;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;

    public class LetstalkService : ILetstalkService
    {
        private const int payloadId = 100;
        private readonly BotConfig botConfig;
        private readonly IHttpClientService httpClientService;
        private readonly ICryptographyService cryptographyService;

        private readonly JsonSerializerSettings DefaultJsonSerializeSetting = new()
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };

        public LetstalkService(
            ICryptographyService cryptographyService,
            IHttpClientService httpClientService,
            BotConfig botConfig)
        {
            this.cryptographyService = cryptographyService;
            this.httpClientService = httpClientService;
            this.botConfig = botConfig;
        }

        public async Task<PushMessageResult> PushMulticastAsync(IEnumerable<string> accountIds, OutgoingMessage message)
        {
            ValidateInputParam(nameof(accountIds), accountIds);
            ValidateInputParam(nameof(message), message);
            ValidateMultiCastMessage(message);

            var token = await GetSignAccessTokenAsync();
            var result = await httpClientService.PostAsync<PushMessageResult>(
                $"bot/v3/message/multicast",
                new Dictionary<string, object>
                {
                    { "msg", message },
                    { "receivers", accountIds.ToArray() },
                    { "forcePush", 1 }
                },
                token,
                encodeData: false);

            return result;
        }

        public async Task<PushMessageResult> PushRoomAsync(string chatroomToken, string message, DateTime? expiredTime = null)
        {
            ValidateInputParam(nameof(chatroomToken), chatroomToken);
            ValidateInputParam(nameof(message), message);
            var outGoingMsg = new OutgoingMessage
            {
                MessageType = MessageTypeNumber.Html,
                Text = message
            };
            var token = await GetSignAccessTokenAsync();
            var result = await httpClientService.PostAsync<PushMessageResult>(
               $"bot/push_chatroom/{botConfig.BotAccountId}",
               new Dictionary<string, object>
               {
                    { "receiver", chatroomToken },
                    { "msg", outGoingMsg }
               },
               token,
               encodeData: false);

            return result ?? new PushMessageResult();
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

        public async Task<PushMessageResult> PushMultiRoomAsync(IEnumerable<string> roomTokenList, OutgoingMessage message)
        {
            ValidateInputParam(nameof(roomTokenList), roomTokenList);
            ValidateInputParam(nameof(message), message);
            ValidateMultiCastMessage(message);

            var token = await GetSignAccessTokenAsync();
            var result = await httpClientService.PostAsync<PushMessageResult>(
                $"bot/v3/message/multi-chatroom",
                new Dictionary<string, object>
                {
                    { "msg", message },
                    { "receivers", roomTokenList }
                },
                token,
                encodeData: false);

            return result;
        }

        private static void ValidateMultiCastMessage(OutgoingMessage message)
        {
            if (message.MessageType == MessageTypeNumber.Text && string.IsNullOrWhiteSpace(message.Text))
            {
                throw new ArgumentNullException(message.Text);
            }

            if ((message.MessageType == MessageTypeNumber.Image && message.MessageType == MessageTypeNumber.FullImage)
                && string.IsNullOrWhiteSpace(message.ImageUrl))
            {
                throw new ArgumentNullException(message.ImageUrl);
            }
        }

        public async Task<string> GetWebhookAsync()
        {
            var result = await httpClientService.GetAsync<WebhookResult>($"bot/get_webhook/{botConfig.BotAccountId}");

            if (result.IsSuccess)
            {
                return result.Url;
            }

            return string.Empty;
        }

        public async Task<BaseResult> SetWebhookAsync(string url)
        {
            ValidateInputParam(nameof(url), url);
            return await httpClientService.PostAsync<BaseResult>(
                $"bot/set_webhook/{botConfig.BotAccountId}",
                new Dictionary<string, object>
                {
                    { "url", url }
                });
        }

        private async Task<PushMessageResult> PushAsync(IEnumerable<string> accountIds, Payload payload, string type = Models.MessageType.Text)
        {
            ValidateInputParam(nameof(accountIds), accountIds);
            ValidateInputParam(nameof(payload), payload);

            var accountIdToLowerList = accountIds.Select(accountId => accountId.ToLowerInvariant());

            payload.UserId = botConfig.BotUserId;
            payload.Time = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            payload.HashType = botConfig.EncryptionMode;
            payload.From = botConfig.BusinessAccountType;
            payload.Type = type;
            payload.Id = payloadId;
            payload.Receiver = accountIdToLowerList.ElementAt(0);
            payload.HashId = botConfig.ChatKeyVersion;

            var result = await httpClientService.PostAsync<PushMessageResult>(
                $"bot/push/{botConfig.BotAccountId}",
                new Dictionary<string, object>
                {
                    { "receiver", accountIdToLowerList },
                    { "payload", JsonConvert.SerializeObject(payload, DefaultJsonSerializeSetting) }
                });

            return result;
        }

        private async Task<string> GetSignAccessTokenAsync()
        {
            var result = await httpClientService.PostAsync<TokenResult>(
                 $"bot/v3/sign-token",
                 new Dictionary<string, object>
                 {
                    { "id", botConfig.BotAccountId },
                    { "token", botConfig.ApiKey }
                 },
                 encodeData: false);
            return result.Token;
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