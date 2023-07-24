namespace GO.Core.Services
{
    using System.Threading.Tasks;
    using GO.Core.Models;

    public interface IBaseMailService
    {
        Task SendMailAsync(MailModel email);

        string ReadTextFile(string filePath);
    }
}