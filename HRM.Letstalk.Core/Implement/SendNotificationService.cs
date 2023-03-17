namespace HRM.Letstalk.Core.Implement
{
    using System.Collections;
    using System.Collections.Generic;
    using HRM.Letstalk.Core.Constants;
    using HRM.Letstalk.Core.Interface;
    using HRM.Letstalk.Core.Models;
    public class SendNotificationService : ISendNotificationService
    {
        private readonly IHttpClientService httpClientService;

        public SendNotificationService(IHttpClientService httpClientService)
        {
            this.httpClientService = httpClientService;
        }


        public JwtToken GetToken(string url, string ursername, string password)
        {
            var baseUrl = $"{url}{ApiEndPoint.Auth}?username={ursername}&password={password}";
            var token = httpClientService.ExecuteGet<JwtToken>(baseUrl);
            return token;
        }

        public void SendNotificationToLetstalk1(string url, string token, Dictionary<string, object> data)
        {
            var baseUrl = url + ApiEndPoint.Notify;
            httpClientService.ExecutePost<object>(baseUrl, token, data);
        }
    }
}