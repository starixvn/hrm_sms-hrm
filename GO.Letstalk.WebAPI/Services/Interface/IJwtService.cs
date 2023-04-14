namespace GO.Letstalk.WebAPI.Services
{
    using System.Collections.Generic;

    public interface IJwtService
    {
        string Decode(string secretKey, string token);

        string Encode(string secretKey, IDictionary<string, object> data);
    }
}