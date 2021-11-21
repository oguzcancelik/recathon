using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace SpotifyGateway.Infrastructure.Extensions
{
    public static class CryptographyExtensions
    {
        public static string Encrypt(this string source, string key)
        {
            using var aes = Aes.Create();
            aes.Key = Encoding.UTF8.GetBytes(key);
            aes.IV = new byte[16];
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

            using var memoryStream = new MemoryStream();
            using var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);
            using (var streamWriter = new StreamWriter(cryptoStream))
            {
                streamWriter.Write(source);
            }

            var array = memoryStream.ToArray();

            return Convert.ToBase64String(array);
        }

        public static string Decrypt(this string source, string key)
        {
            var buffer = Convert.FromBase64String(source);

            using var aes = Aes.Create();
            aes.Key = Encoding.UTF8.GetBytes(key);
            aes.IV = new byte[16];
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

            using var memoryStream = new MemoryStream(buffer);
            using var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
            using var streamReader = new StreamReader(cryptoStream);

            return streamReader.ReadToEnd();
        }

        public static string Mask(this string source)
        {
            if (source == null || source.Length < 2)
            {
                return source;
            }

            var value = Enumerable.Range(0, source.Length).Select(x => x % 3 != 0 ? source[x] : '*');

            return string.Concat(value);
        }
    }
}