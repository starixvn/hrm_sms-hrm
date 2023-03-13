namespace HRM.Letstalk.Client.Services.Interface
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System;
    using HRM.Letstalk.Client.Models;
    using HRM.Letstalk.Client.Models.Result;
    public interface ILetstalkService
    {
        Task<PushMessageResult> PushAsync(IEnumerable<string> accountIds, Payload payload);

        Task<PushMessageResult> PushAsync(IEnumerable<string> accountIds, string message, DateTime? expiredTime = null);

        Task<PushMessageResult> PushAsync(string accountId, string message, DateTime? expiredTime = null);

        Task<PushMessageResult> PushAsync(IEnumerable<string> accountIds, Message message, DateTime? expiredTime = null);

        Task<BaseResult> PushRoomAsync(string chatroomToken, string message, DateTime? expiredTime = null);

        Task<string> GetWebhookAsync();

        Task<BaseResult> SetWebhookAsync(string url);
    }
}
