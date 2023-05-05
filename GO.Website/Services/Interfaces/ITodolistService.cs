namespace GO.Website.Services
{
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading.Tasks;
    using GO.Website.Models;
    using GO.Website.Models.Todolist;

    public interface ITodolistService
    {
        Task<IEnumerable<UserSite>> GetUserSiteByUserIdAsync(int userId);

        Task<string> GetTokenAsync(ApiSiteSetting apiSiteSetting);

        Task<IList<TodolistItem>> GetTodolistAsync(IList<UserSite> usersites, IList<ApiSiteSetting> apiSiteSettings);

        Task<T> GetAsync<T>(string requestUrl, string token = null);

        Task<T> PostAsync<T>(string requestUrl, HttpContent content = null, string token = null);
    }
}