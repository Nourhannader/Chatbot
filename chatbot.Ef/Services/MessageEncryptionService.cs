using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Interfaces.Services;

namespace chatbot.Ef.Services
{
    public class MessageEncryptionService : IMessageEncryptionService
    {
        public string Decrypt(string cipherText, string key)
        {
            var data=Convert.FromBase64String(cipherText);
            using var aes = Aes.Create();

            aes.Key =SHA256.HashData(Encoding.UTF8.GetBytes(key));
            var iv = data.Take(16).ToArray();
            var encrypted = data.Skip(16).ToArray();
            var IV = iv;
            using var decryptor =aes.CreateDecryptor();

            var decrypted =decryptor.TransformFinalBlock(encrypted,0,encrypted.Length);

            return Encoding.UTF8.GetString(decrypted);
        }

        public string Encrypt(string plainText, string key)
        {
            using var aes=Aes.Create();
            aes.Key=SHA256.HashData(Encoding.UTF8.GetBytes(key));
            aes.GenerateIV();
            using var encryptor=aes.CreateEncryptor();
            var input = Encoding.UTF8.GetBytes(plainText);
            var encrypted=encryptor.TransformFinalBlock(input,0, input.Length);
            var result=aes.IV.Concat(encrypted).ToArray();
            return Convert.ToBase64String(result);
        }
    }
}
