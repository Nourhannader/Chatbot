using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chatbot.Core.Interfaces.Services
{
    public interface IMessageEncryptionService
    {
        string Encrypt(string plainText, string key);

        string Decrypt(string cipherText, string key);
    }
}
