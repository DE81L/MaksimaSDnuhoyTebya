using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace MaksimaSDnuhoyTebya
{
    public static class EncryptionHelper
    {
        private static readonly string EncryptionKey = "MaksimaSDnuhoyTebya123"; // Длина ключа должна быть корректной

        public static string Encrypt(string plainText)
        {
            using (var aes = Aes.Create())
            {
                var key = Encoding.UTF8.GetBytes(EncryptionKey);
                Array.Resize(ref key, 32); // Убедитесь, что длина ключа правильная
                aes.Key = key;

                using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                {
                    using (var ms = new MemoryStream())
                    {
                        ms.Write(aes.IV, 0, aes.IV.Length); // Пишем IV в начало потока

                        using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                        {
                            using (var sw = new StreamWriter(cs))
                            {
                                sw.Write(plainText);
                            }
                        }

                        var encryptedContent = ms.ToArray();
                        return Convert.ToBase64String(encryptedContent);
                    }
                }
            }
        }

        public static string Decrypt(string encryptedText)
        {
            var fullCipher = Convert.FromBase64String(encryptedText);

            using (var aes = Aes.Create())
            {
                var iv = new byte[aes.BlockSize / 8];
                var cipher = new byte[fullCipher.Length - iv.Length];

                Buffer.BlockCopy(fullCipher, 0, iv, 0, iv.Length);
                Buffer.BlockCopy(fullCipher, iv.Length, cipher, 0, cipher.Length);

                var key = Encoding.UTF8.GetBytes(EncryptionKey);
                Array.Resize(ref key, 32); // Убедитесь, что длина ключа правильная
                aes.Key = key;
                aes.IV = iv;

                using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                {
                    using (var ms = new MemoryStream(cipher))
                    {
                        using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                        {
                            using (var sr = new StreamReader(cs))
                            {
                                return sr.ReadToEnd();
                            }
                        }
                    }
                }
            }
        }
    }
}
