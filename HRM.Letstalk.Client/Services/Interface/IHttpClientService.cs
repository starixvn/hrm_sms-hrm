namespace HRM.Letstalk.Client.Services.Interface
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IHttpClientService
    {
        Task<T> ExecuteGetAsync<T>(string endPoint, IDictionary<string, object> data = null, string signToken = "", bool encodeData = true) where T : new();

        Task<T> ExecutePostAsync<T>(string endPoint, IDictionary<string, object> data, string signToken = "", bool encodeData = true) where T : new();

        Task<T> ExecutePutAsync<T>(string endPoint, IDictionary<string, object> data, string signToken = "", bool encodeData = true) where T : new();

        Task<T> ExecuteDeleteAsync<T>(string endPoint, IDictionary<string, object> data, string signToken = "", bool encodeData = true) where T : new();
    }
}
