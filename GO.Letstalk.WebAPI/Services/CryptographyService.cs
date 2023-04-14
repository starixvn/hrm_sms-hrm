namespace GO.Letstalk.WebAPI.Services
{
    using System;
    using System.IO;
    using System.Security.Cryptography;
    using System.Text;

    public class CryptographyService : ICryptographyService
    {
        public string Decrypt(string encryptedMessage, string key, string salt)
        {
            if (string.IsNullOrWhiteSpace(encryptedMessage))
            {
                throw new ArgumentNullException(nameof(encryptedMessage));
            }

            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (string.IsNullOrWhiteSpace(salt))
            {
                throw new ArgumentNullException(nameof(salt));
            }

            string plaintext = null;

            using (var rijAlg = new RijndaelManaged())
            {
                rijAlg.Key = ConvertHexStringToByte(key);
                rijAlg.IV = ComputeSalt(salt);

                var decryptor = rijAlg.CreateDecryptor(rijAlg.Key, rijAlg.IV);

                using (var msDecrypt = new MemoryStream(Convert.FromBase64String(encryptedMessage)))
                {
                    using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        var srDecrypt = new StreamReader(csDecrypt);
                        plaintext = srDecrypt.ReadToEnd();
                    }
                }
            }

            return plaintext;
        }

        public string Encrypt(string message, string key, string salt)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentNullException(nameof(message));
            }

            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (string.IsNullOrWhiteSpace(salt))
            {
                throw new ArgumentNullException(nameof(salt));
            }

            byte[] encrypted;

            using (var rijAlg = new RijndaelManaged())
            {
                rijAlg.Key = ConvertHexStringToByte(key);
                rijAlg.IV = ComputeSalt(salt);

                var encryptor = rijAlg.CreateEncryptor(rijAlg.Key, rijAlg.IV);

                using (var msEncrypt = new MemoryStream())
                {
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (var swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(message);
                        }

                        encrypted = msEncrypt.ToArray();
                    }
                }
            }

            return Convert.ToBase64String(encrypted);
        }

        public string Hash(string message)
        {
            var messageBytes = Encoding.UTF8.GetBytes(message);
            var sha = new SHA1CryptoServiceProvider();
            var result = sha.ComputeHash(messageBytes);

            return ConvertHexByteToString(result);
        }

        private static string ConvertHexByteToString(byte[] data)
            => BitConverter.ToString(data).Replace("-", "");

        private static byte[] ConvertHexStringToByte(string hex)
        {
            const int numStep = 2;
            const int fromBase = 16;

            int NumberChars = hex.Length;
            byte[] bytes = new byte[NumberChars / numStep];
            for (int i = 0; i < NumberChars; i += numStep)
            {
                bytes[i / numStep] = Convert.ToByte(hex.Substring(i, numStep), fromBase);
            }

            return bytes;
        }

        private static byte[] ComputeSalt(string salt)
        {
            MD5 md5 = MD5.Create();
            byte[] source = Encoding.Default.GetBytes(salt);
            return md5.ComputeHash(source);
        }
    }
}