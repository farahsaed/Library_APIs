using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Library_APIs
{
    public static class Encryption
    {
        
        private static readonly byte[] Key = Encoding.ASCII.GetBytes("mikviVRQZclNP9KdHtAEctzt");
        private static readonly byte[] IV = Encoding.ASCII.GetBytes("YourIV12");

        public static string Encrypt(string plainText)
        {
            byte[] encryptedData;
            using (var des = TripleDES.Create())
            {
                des.Key = Key;
                des.IV = IV;

                using (var encryptor = des.CreateEncryptor())
                using (var memoryStream = new MemoryStream())
                {
                    using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        using (var streamWriter = new StreamWriter(cryptoStream))
                        {
                            streamWriter.Write(plainText);
                        }
                    }

                    encryptedData = memoryStream.ToArray();
                }
            }

            return Convert.ToBase64String(encryptedData);
        }

        public static string Decrypt(string encryptedText)
        {
            byte[] cipherData = Convert.FromBase64String(encryptedText);
            string decryptedText;

            using (var des = TripleDES.Create())
            {
                des.Key = Key;
                des.IV = IV;

                using (var decryptor = des.CreateDecryptor())
                using (var memoryStream = new MemoryStream(cipherData))
                using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                using (var streamReader = new StreamReader(cryptoStream))
                {
                    decryptedText = streamReader.ReadToEnd();
                }
            }

            return decryptedText;
        }
    }
}