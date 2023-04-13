namespace Fanex.NexHR.Services
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Threading;
    using Fanex.NexHR.Core;
    using Newtonsoft.Json;

    public class PushNotificationService : IPushNotificationService
    {
        private readonly log4net.ILog logger = log4net.LogManager.GetLogger(typeof(PushNotificationService));
        private static readonly HttpClient httpClient = new HttpClient();

        public void SendNotiRefreshTodolist()
        {
            if (ConfigSettings.GO_Todolist_EnableNotiRefreshHrm)
            {
                try
                {
                    var token = GetToken();
                    using (var request = CreateRequest(ConfigSettings.GO_Todolist_NotiRefreshHrmUrl, HttpMethod.Post, token))
                    {
                        using (var response = httpClient.SendAsync(request, HttpCompletionOption.ResponseContentRead, CancellationToken.None).Result)
                        {
                            if (!response.IsSuccessStatusCode)
                            {
                                var message = response.Content.ReadAsStringAsync().Result;
                                logger.Error(string.Format("StatusCode: {0}\r\nMessage: {1}", response.StatusCode, message));
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger.Error(ex.Message);
                }
            }
        }

        private string GetToken()
        {
            var loginParam = new Dictionary<string, string>
            {
                { "Username", ConfigSettings.GO_Token_Username },
                { "Password", ConfigSettings.GO_Token_Password }
            };

            try
            {
                using (var request = CreateRequest(ConfigSettings.GO_Token_Url, HttpMethod.Post))
                {
                    var encodedData = JsonConvert.SerializeObject(loginParam);
                    var content = new StringContent(encodedData, Encoding.UTF8, "application/json");
                    request.Content = content;

                    using (var response = httpClient.SendAsync(request, HttpCompletionOption.ResponseContentRead, CancellationToken.None).Result)
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            var token = response.Content.ReadAsStringAsync().Result;
                            return token;
                        }
                        else
                        {
                            var message = response.Content.ReadAsStringAsync().Result;
                            logger.Error(string.Format("StatusCode: {0}\r\nMessage: {1}", response.StatusCode, message));

                            return default;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);

                return default;
            }
        }

        private static HttpRequestMessage CreateRequest(string endPoint, HttpMethod method, string jwttoken = null)
        {
            var url = new Uri(endPoint);
            var request = new HttpRequestMessage(method, url);
            request.Headers.Accept.Clear();
            if (!string.IsNullOrWhiteSpace(jwttoken))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwttoken);
            }

            return request;
        }
    }
}