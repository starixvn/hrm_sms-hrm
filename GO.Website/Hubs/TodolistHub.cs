namespace GO.Website.Hubs
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.SignalR;

    public class TodolistHub : Hub<ITodolistClient>
    {
        public async Task RefreshHrm()
        {
            await Clients.All.RefreshHrm();
        }
    }
}