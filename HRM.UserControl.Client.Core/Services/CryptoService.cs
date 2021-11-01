namespace HRM.UserControl.Client.Core.Services
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

        protected internal virtual string GenerateKey()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%&";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, KeyLength).Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}