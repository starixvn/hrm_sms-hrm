namespace HRM.Letstalk.Core.Interface
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using HRM.Letstalk.Core.Models;
    public interface ISendNotificationService
    {
        JwtToken GetToken(string url, string ursername, string password);
        void SendNotificationToLetstalk1(string url, string token, Dictionary<string, object> data);
    }
}
