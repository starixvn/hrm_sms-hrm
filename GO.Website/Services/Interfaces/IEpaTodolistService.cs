namespace GO.Website.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using GO.Website.Models;
    using GO.Website.Models.Todolist;

    public interface IEpaTodolistService
    {
        Task<IList<TodolistItem>> GetByUserIdAsync(int userId, IEnumerable<UserSite> userSites);
    }
}