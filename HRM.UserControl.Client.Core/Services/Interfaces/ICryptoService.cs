namespace HRM.UserControl.Client.Core.Services
{
    public interface ICryptoService
    {
        string GenerateRijndaelKeyIV();

        string Encrypt(string plainText, string rijndaelKey, string rijndaelIV);

        string Decrypt(string cipherText, string rijndaelKey, string rijndaelIV);
    }
}