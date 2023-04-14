namespace GO.Letstalk.WebAPI.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IHttpClientService
    {
        Task<T> GetAsync<T>(string endPoint, IDictionary<string, object> data = null, string signToken = "", bool encodeData = true) where T : new();

        Task<T> PostAsync<T>(string endPoint, IDictionary<string, object> data, string signToken = "", bool encodeData = true) where T : new();

        Task<T> PutAsync<T>(string endPoint, IDictionary<string, object> data, string signToken = "", bool encodeData = true) where T : new();

        Task<T> DeleteAsync<T>(string endPoint, IDictionary<string, object> data, string signToken = "", bool encodeData = true) where T : new();
    }
}