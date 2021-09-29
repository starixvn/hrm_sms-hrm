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

    internal class BaseHttpClient
    {
        private static readonly HttpClient httpClient = new HttpClient();
        private readonly IUserControlSettings ucSettings;
        private readonly IUCSession ucSession;

        public BaseHttpClient()
        {
            ucSettings = UserControlClientManager.UserControlSettings;
            ucSession = UserControlClientManager.UserControlSession;
        }

        protected T Get<T>(string requestUrl, IDictionary<string, object> data = null)
            => Send<T>(requestUrl, HttpMethod.Get, data);

        protected T Post<T>(string requestUrl, IDictionary<string, object> data)
            => Send<T>(requestUrl, HttpMethod.Post, data);

        protected async Task<T> GetAsync<T>(string requestUrl, IDictionary<string, object> data = null)
            => await SendAsync<T>(requestUrl, HttpMethod.Get, data);

        protected async Task<T> PostAsync<T>(string requestUrl, IDictionary<string, object> data)
            => await SendAsync<T>(requestUrl, HttpMethod.Post, data);

        private async Task<T> SendAsync<T>(string endPoint, HttpMethod method, IDictionary<string, object> data)
        {
            ValidateEndPoint(endPoint);

            using (var request = CreateRequest(endPoint, method))
            {
                if (data != null)
                {
                    var encodedData = JsonConvert.SerializeObject(data);
                    var content = new StringContent(encodedData, Encoding.UTF8, "application/json");
                    request.Content = content;
                }
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

        private T Send<T>(string endPoint, HttpMethod method, IDictionary<string, object> data)
        {
            ValidateEndPoint(endPoint);

            using (var request = CreateRequest(endPoint, method))
            {
                if (data != null)
                {
                    var encodedData = JsonConvert.SerializeObject(data);
                    var content = new StringContent(encodedData, Encoding.UTF8, "application/json");
                    request.Content = content;
                }
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

        private static void ValidateEndPoint(string endPoint)
        {
            if (string.IsNullOrWhiteSpace(endPoint))
            {
                throw new ArgumentNullException(nameof(endPoint));
            }
        }

        private HttpRequestMessage CreateRequest(string endPoint, HttpMethod method)
        {
            Uri url = new Uri($"{ucSettings.ServiceUrl}{endPoint}");
            var request = new HttpRequestMessage(method, url);
            request.Headers.Accept.Clear();
            string siteIdentity = GenerateSiteIdentity();
            request.Headers.Add("SiteIdentity", $"Bearer {siteIdentity}");
            if (!string.IsNullOrWhiteSpace(ucSession.JwtToken))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", ucSession.JwtToken);
            }

            return request;
        }

        private string GenerateSiteIdentity()
        {
            var data = new Dictionary<string, object>
            {
                {nameof(ucSettings.SiteId), ucSettings.SiteId },
                {nameof(ucSettings.SiteName), ucSettings.SiteName},
                {nameof(ucSettings.AccessToken), ucSettings.AccessToken }
            };
            return JWT.Encode(data, Encoding.UTF8.GetBytes(ucSettings.AccessToken), JwsAlgorithm.HS256);
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
                throw new Exception(message);
            }
        }
    }
}