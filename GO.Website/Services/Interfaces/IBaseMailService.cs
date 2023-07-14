namespace GO.Website.Services
{
    using System.Threading.Tasks;
    using GO.Website.Models;

    public interface IBaseMailService
    {
        Task SendMailAsync(MailModel email);

        string ReadTextFile(string filePath);
    }
}