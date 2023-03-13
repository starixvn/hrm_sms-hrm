namespace HRM.Letstalk.Client.Services.Interface
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IHttpClientService
    {
        Task<T> ExecuteGetAsync<T>(string endPoint, IDictionary<string, object> data = null) where T : new();

        Task<T> ExecutePostAsync<T>(string endPoint, IDictionary<string, object> data) where T : new();

        Task<T> ExecutePutAsync<T>(string endPoint, IDictionary<string, object> data) where T : new();

        Task<T> ExecuteDeleteAsync<T>(string endPoint, IDictionary<string, object> data) where T : new();
    }
}
