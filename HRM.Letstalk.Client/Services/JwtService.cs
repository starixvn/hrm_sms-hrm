namespace HRM.Letstalk.Client.Services
{
    using System.Collections.Generic;
    using System.Text;
    using HRM.Letstalk.Client.Services.Interface;
    using Jose;

    public class JwtService : IJwtService
    {
        public string Decode(string secretKey, string token)
        {
            var signingKey = Encoding.UTF8.GetBytes(secretKey);

            var encodedPayload = JWT.Decode(
                token,
                signingKey,
                JwsAlgorithm.HS512);

            return encodedPayload;
        }

        public string Encode(string secretKey, IDictionary<string, object> data)
        {
            var signingKey = Encoding.UTF8.GetBytes(secretKey);

            var encodedPayload = JWT.Encode(
                data,
                signingKey,
                JwsAlgorithm.HS512);

            return encodedPayload;
        }
    }
}
