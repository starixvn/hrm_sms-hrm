namespace HRM.UserControl.Core.Services
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;

    public interface ICryptoService
    {
        string GenerateRijndaelKeyIV();

        string Encrypt(string plainText, string rijndaelKey, string rijndaelIV);

        string Decrypt(string cipherText, string rijndaelKey, string rijndaelIV);

        bool CompareStringToHash(string plainText, string encrypt, int saltSize = 8);
    }

    public class CryptoService : ICryptoService
    {
        private const byte FeedbackSize = 128;
        private const byte KeyLength = 16;

        public string GenerateRijndaelKeyIV()
        {
            return string.Format("{0}:{1}", GenerateKey(), GenerateKey());
        }

        public string Encrypt(string plainText, string rijndaelKey, string rijndaelIV)
        {
            if (!string.IsNullOrEmpty(plainText))
            {
                using (var rijAlg = new RijndaelManaged())
                {
                    rijAlg.Mode = CipherMode.CBC;
                    rijAlg.Padding = PaddingMode.PKCS7;
                    rijAlg.FeedbackSize = FeedbackSize;
                    rijAlg.Key = Encoding.UTF8.GetBytes(rijndaelKey);
                    rijAlg.IV = Encoding.UTF8.GetBytes(rijndaelIV);

                    var encryptor = rijAlg.CreateEncryptor();
                    using (var msEncrypt = new MemoryStream())
                    {
                        using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                        {
                            using (var srEncrypt = new StreamWriter(csEncrypt))
                            {
                                srEncrypt.Write(plainText);
                            }

                            return Convert.ToBase64String(msEncrypt.ToArray());
                        }
                    }
                }
            }

            return string.Empty;
        }

        public string Decrypt(string cipherText, string rijndaelKey, string rijndaelIV)
        {
            if (!string.IsNullOrEmpty(cipherText))
            {
                using (var rijAlg = new RijndaelManaged())
                {
                    rijAlg.Mode = CipherMode.CBC;
                    rijAlg.Padding = PaddingMode.PKCS7;
                    rijAlg.FeedbackSize = FeedbackSize;
                    rijAlg.Key = Encoding.UTF8.GetBytes(rijndaelKey);
                    rijAlg.IV = Encoding.UTF8.GetBytes(rijndaelIV);
                    var decryptor = rijAlg.CreateDecryptor();
                    using (var msDecrypt = new MemoryStream(Convert.FromBase64String(cipherText)))
                    {
                        using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                        {
                            using (var srDecrypt = new StreamReader(csDecrypt))
                            {
                                return srDecrypt.ReadToEnd();
                            }
                        }
                    }
                }
            }

            return string.Empty;
        }

        public bool CompareStringToHash(string plainText, string encrypt, int saltSize = 8)
        {
            byte[] encryptBuffer = Convert.FromBase64String(encrypt);
            int encryptLength = encryptBuffer.Length;

            byte[] saltBuffer = new byte[saltSize];
            Array.Copy(encryptBuffer, encryptLength - saltSize, saltBuffer, 0, saltSize);

            string hash = ComputeHash(plainText, saltBuffer);
            return string.Equals(encrypt, hash);
        }

        protected internal virtual string GenerateKey()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%&";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, KeyLength).Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private static string ComputeHash(string plainText, byte[] salt)
        {
            byte[] data = Encoding.UTF8.GetBytes(plainText);
            int dataLength = data.Length;

            int saltLength = salt.Length;

            byte[] dataSalt = new byte[dataLength + saltLength];

            Array.Copy(data, dataSalt, dataLength);
            Array.Copy(salt, 0, dataSalt, dataLength, saltLength);

            HashAlgorithm hash = new SHA1Managed();
            byte[] encrypt = hash.ComputeHash(dataSalt);
            int encryptLength = encrypt.Length;

            byte[] encryptSalt = new byte[encryptLength + saltLength];
            Array.Copy(encrypt, encryptSalt, encryptLength);
            Array.Copy(salt, 0, encryptSalt, encryptLength, saltLength);

            return Convert.ToBase64String(encryptSalt);
        }
    }
}