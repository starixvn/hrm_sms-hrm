namespace GO.Letstalk.WebAPI.Services
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using GO.Letstalk.WebAPI.Models;
    using Newtonsoft.Json;

    public class HttpClientService : IHttpClientService
    {
        private const string JsonContentType = "application/json";
        private const string PlainTextContentType = "text/plain";
        private const int ExpiredTokenPeriodInMiliseconds = 5000;
        private readonly IHttpClientFactory clientFactory;
        private readonly IJwtService jwtService;
        private readonly BotConfig botConfig;

        public HttpClientService(IHttpClientFactory clientFactory, IJwtService jwtService, BotConfig botConfig)
        {
            this.clientFactory = clientFactory;
            this.jwtService = jwtService;
            this.botConfig = botConfig;
        }

        public Task<T> GetAsync<T>(string endPoint, IDictionary<string, object> data = null, string signToken = "", bool encodeData = true) where T : new()
            => SendHttpClientAsync<T>(endPoint, HttpMethod.Get, data, signToken, encodeData);

        public Task<T> PostAsync<T>(string endPoint, IDictionary<string, object> data, string signToken = "", bool encodeData = true) where T : new()
            => SendHttpClientAsync<T>(endPoint, HttpMethod.Post, data, signToken, encodeData);

        public Task<T> PutAsync<T>(string endPoint, IDictionary<string, object> data, string signToken = "", bool encodeData = true) where T : new()
            => SendHttpClientAsync<T>(endPoint, HttpMethod.Put, data, signToken, encodeData);

        public Task<T> DeleteAsync<T>(string endPoint, IDictionary<string, object> data, string signToken = "", bool encodeData = true) where T : new()
            => SendHttpClientAsync<T>(endPoint, HttpMethod.Delete, data, signToken, encodeData);

        private async Task<T> SendHttpClientAsync<T>(string endPoint, HttpMethod method, IDictionary<string, object> data, string signToken, bool encodeData)
            where T : new()
        {
            ValidateEndPoint(endPoint);

            using (var request = CreateRequest(endPoint, method, signToken))
            {
                if (data != null)
                {
                    StringContent content;
                    if (encodeData)
                    {
                        var encodedData = Encode(data);
                        content = new StringContent(encodedData, Encoding.UTF8, PlainTextContentType);
                    }
                    else
                    {
                        var jsonData = JsonConvert.SerializeObject(data);
                        content = new StringContent(jsonData, Encoding.UTF8, JsonContentType);
                    }

                    request.Content = content;
                }

                var client = clientFactory.CreateClient();
                using (var response = await client.SendAsync(request, HttpCompletionOption.ResponseContentRead, CancellationToken.None))
                {
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

        private HttpRequestMessage CreateRequest(string endPoint, HttpMethod method, string signToken)
        {
            var url = new Uri($"{botConfig.ApiEndpoint}{endPoint}");
            string token = string.IsNullOrWhiteSpace(signToken) ? EncodeBase64(botConfig.ApiKey) : signToken;

            var request = new HttpRequestMessage(method, url);
            request.Headers.Accept.Clear();
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            return request;
        }

        private static void ValidateEndPoint(string endPoint)
        {
            if (string.IsNullOrWhiteSpace(endPoint))
            {
                throw new ArgumentNullException(nameof(endPoint));
            }
        }

        private string Encode(IDictionary<string, object> data)
        {
            var totalMilisecondNow = DateTimeOffset.Now.ToUnixTimeMilliseconds();

            data.Add("iat", totalMilisecondNow);
            data.Add("exp", totalMilisecondNow + ExpiredTokenPeriodInMiliseconds);

            var result = jwtService.Encode(botConfig.SecureKey, data);
            return result;
        }

        private static string EncodeBase64(string input)
            => Convert.ToBase64String(Encoding.UTF8.GetBytes(input));
    }
}