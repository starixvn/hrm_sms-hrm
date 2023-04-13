namespace GO.Website.Hubs
{
    using System.Threading.Tasks;

    public interface ITodolistClient
    {
        Task RefreshHrm();
    }
}