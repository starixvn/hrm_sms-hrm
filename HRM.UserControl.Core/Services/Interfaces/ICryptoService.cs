namespace HRM.UserControl.Core.Services
{
    public interface ICryptoService
    {
        string GenerateRijndaelKeyIV();

        string Encrypt(string plainText, string rijndaelKey, string rijndaelIV);

        string Decrypt(string cipherText, string rijndaelKey, string rijndaelIV);

        bool CompareStringToHash(string plainText, string encrypt, int saltSize = 8);
    }
}