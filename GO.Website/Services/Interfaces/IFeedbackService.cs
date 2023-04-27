namespace GO.Website.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using GO.Website.Models.Todolist;

    public interface IFeedbackService
    {
        Task<IList<TodolistItem>> GetTodolistByUserIdAsync(int userId);
    }
}