namespace HRM.Letstalk.Client.Services.Interface
{
    public interface ICryptographyService
    {
        string Decrypt(string encryptedMessage, string key, string salt);

        string Encrypt(string message, string key, string salt);

        string Hash(string message);
    }
}
