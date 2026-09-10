using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.Interfaces.Services;

namespace chatbot.Ef.Services
{
    public class TokenHashService : ITokenHashService
    {
        public string Hash(string token)
        {
           using var sha=SHA256.Create();
            var bytes=Encoding.UTF8.GetBytes(token);
            var hash=sha.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        public bool Verify(string token, string hash)
        {
            var tokenHash = Hash(token);

            return CryptographicOperations
                .FixedTimeEquals(
                    Convert.FromBase64String(tokenHash),
                    Convert.FromBase64String(hash));
        }
    }
}
