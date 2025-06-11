using Investigator.Services.IServices;
using System.Security.Cryptography;
using System.Text;

namespace Investigator.Services
{
    public class HmacGenerator:IHmacGenerator
    {
        private readonly byte[] key;
        public HmacGenerator()
        {
            key = GenerateSecureKey();
        }
        public string GenerateHmac(int message)
        {
            using (var hmac = new HMACSHA256(key))
            {
                byte[] messageBytes = Encoding.UTF8.GetBytes(message.ToString());
                byte[] hash = hmac.ComputeHash(messageBytes);
                return BitConverter.ToString(hash).Replace("-", "").ToLower();
            }
        }
        private static byte[] GenerateSecureKey()
        {
            var key = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(key);
            }
            return key;
        }
    }
}
