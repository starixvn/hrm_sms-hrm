namespace Fanex.NexHR.Core.Security
{
    using Fanex.NexHR.Core.Enums;
    using System;
    using System.Security.Cryptography;
    using System.Text;

    public static class Encryptor
    {
        public static string ComputeHash(HashProvider hashAlgorithm, string plainText, int saltSize = 8)
        {
            byte[] salt = Random.RNGCrypto(saltSize);
            return ComputeHash(hashAlgorithm, plainText, salt);
        }

        public static bool CompareStringToHash(HashProvider hashAlgorithm, string plainText, string encrypt, int saltSize = 8)
        {
            byte[] encryptBuffer = Convert.FromBase64String(encrypt);
            int encryptLength = encryptBuffer.Length;

            byte[] saltBuffer = new byte[saltSize];
            Array.Copy(encryptBuffer, encryptLength - saltSize, saltBuffer, 0, saltSize);

            string hash = ComputeHash(hashAlgorithm, plainText, saltBuffer);
            return string.Equals(encrypt, hash);
        }

        private static HashAlgorithm GetHashProvider(HashProvider hashAlgorithm)
        {
            HashAlgorithm hash;
            switch (hashAlgorithm)
            {
                case HashProvider.Sha1:
                    hash = new SHA1Managed();
                    break;

                case HashProvider.Sha256:
                    hash = new SHA256Managed();
                    break;

                case HashProvider.Sha384:
                    hash = new SHA384Managed();
                    break;

                case HashProvider.Sha512:
                    hash = new SHA512Managed();
                    break;

                default:
                    hash = new MD5CryptoServiceProvider();
                    break;
            }

            return hash;
        }

        private static string ComputeHash(HashProvider hashAlgorithm, string plainText, byte[] salt)
        {
            byte[] data = Encoding.UTF8.GetBytes(plainText);
            int dataLength = data.Length;

            int saltLength = salt.Length;

            byte[] dataSalt = new byte[dataLength + saltLength];

            Array.Copy(data, dataSalt, dataLength);
            Array.Copy(salt, 0, dataSalt, dataLength, saltLength);

            byte[] encrypt = GetHashProvider(hashAlgorithm).ComputeHash(dataSalt);
            int encryptLength = encrypt.Length;

            byte[] encryptSalt = new byte[encryptLength + saltLength];
            Array.Copy(encrypt, encryptSalt, encryptLength);
            Array.Copy(salt, 0, encryptSalt, encryptLength, saltLength);

            return Convert.ToBase64String(encryptSalt);
        }
    }
}