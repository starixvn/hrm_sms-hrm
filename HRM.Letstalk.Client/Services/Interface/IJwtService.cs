using System.Collections.Generic;

namespace HRM.Letstalk.Client.Services.Interface
{
    public interface IJwtService
    {
        string Decode(string secretKey, string token);

        string Encode(string secretKey, IDictionary<string, object> data);
    }
}
