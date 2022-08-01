namespace HRM.UserControl.Client.Core.Client
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using HRM.UserControl.Client.Core;
    using HRM.UserControl.Client.Core.Configurations;
    using HRM.UserControl.Client.Core.Session;
    using Jose;
    using Newtonsoft.Json;

    public class BaseHttpClient
    {
        private static readonly HttpClient httpClient = new HttpClient();
        private readonly IUserControlSettings settings;
        private readonly IUserControlSession session;

        public BaseHttpClient()
        {
            settings = UserControlClientManager.UserControlSettings;
            session = UserControlClientManager.UserControlSession;
        }

        protected T Get<T>(string requestUrl)
        {
            ValidateEndPoint(requestUrl);
            using (var request = CreateRequest(requestUrl, HttpMethod.Get))
            {
                using (var response = httpClient.SendAsync(request, HttpCompletionOption.ResponseContentRead, CancellationToken.None).Result)
                {
                    ThrowExceptionWhenNotSuccess(response);
                    if (response.IsSuccessStatusCode)
                    {
                        var responseData = response.Content.ReadAsStringAsync().Result;
                        var result = JsonConvert.DeserializeObject<T>(responseData);

                        return result;
                    }

                    return default;
                }
            }
        }    

        protected async Task<T> GetAsync<T>(string requestUrl)
        {
            ValidateEndPoint(requestUrl);
            using (var request = CreateRequest(requestUrl, HttpMethod.Get))
            {
                using (var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseContentRead, CancellationToken.None))
                {
                    ThrowExceptionWhenNotSuccess(response);
                    if (response.IsSuccessStatusCode)
                    {
                        var responseData = await response.Content.ReadAsStringAsync();
                        var result = JsonConvert.DeserializeObject<T>(responseData);

                        return result;
                    }

                    return default;
                }
            }
        }

        protected TOutput Post<TInput, TOutput>(string requestUrl, TInput data)
        {
            ValidateEndPoint(requestUrl);
            using (var request = CreateRequest(requestUrl, HttpMethod.Post))
            {
                var encodedData = JsonConvert.SerializeObject(data);
                var content = new StringContent(encodedData, Encoding.UTF8, "application/json");
                request.Content = content;
                using (var response = httpClient.SendAsync(request, HttpCompletionOption.ResponseContentRead, CancellationToken.None).Result)
                {
                    ThrowExceptionWhenNotSuccess(response);

                    if (response.IsSuccessStatusCode)
                    {
                        var responseData = response.Content.ReadAsStringAsync().Result;
                        var result = JsonConvert.DeserializeObject<TOutput>(responseData);

                        return result;
                    }

                    return default;
                }
            }
        }

        protected async Task<TOutput> PostAsync<TInput, TOutput>(string requestUrl, TInput data)
        {
            ValidateEndPoint(requestUrl);
            using (var request = CreateRequest(requestUrl, HttpMethod.Post))
            {
                var encodedData = JsonConvert.SerializeObject(data);
                var content = new StringContent(encodedData, Encoding.UTF8, "application/json");
                request.Content = content;
                using (var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseContentRead, CancellationToken.None))
                {
                    ThrowExceptionWhenNotSuccess(response);
                    if (response.IsSuccessStatusCode)
                    {
                        var responseData = await response.Content.ReadAsStringAsync();
                        var result = JsonConvert.DeserializeObject<TOutput>(responseData);

                        return result;
                    }

                    return default;
                }
            }
        }

        private static void ValidateEndPoint(string endPoint)
        {
            if (string.IsNullOrWhiteSpace(endPoint))
            {
                throw new ArgumentNullException(nameof(endPoint));
            }
        }

        private HttpRequestMessage CreateRequest(string endPoint, HttpMethod method)
        {
            Uri url = new Uri($"{settings.ServiceUrl}{endPoint}");
            var request = new HttpRequestMessage(method, url);
            request.Headers.Accept.Clear();
            string siteIdentity = GenerateSiteIdentity();
            request.Headers.Add("SiteIdentity", $"Bearer {siteIdentity}");
            if (session.JwtToken != null && !string.IsNullOrWhiteSpace(session.JwtToken.Token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", session.JwtToken.Token);
            }

            return request;
        }

        private string GenerateSiteIdentity()
        {
            var data = new Dictionary<string, object>
            {
                {nameof(settings.SiteId), settings.SiteId },
                {nameof(settings.AccessToken), settings.AccessToken }
            };
            return JWT.Encode(data, Encoding.UTF8.GetBytes(settings.AccessToken), JwsAlgorithm.HS256);
        }

        private void ThrowExceptionWhenNotSuccess(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    return;
                }

                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    throw new UnauthorizedAccessException();
                }

                var message = response.Content.ReadAsStringAsync().Result;
                var exception = new Exception(message);
                throw exception;
            }
        }
    }
}