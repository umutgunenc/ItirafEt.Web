using System.Security.Cryptography;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;

namespace ItirafEt.Api.Services
{
    public static class PasswordResetTokenHelperService
    {
        public static string GenerateRawToken(int bytes = 64)
        {
            return WebEncoders.Base64UrlEncode(RandomNumberGenerator.GetBytes(bytes));
        }

        public static string HashToken(string rawToken)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(rawToken));
            return Convert.ToHexString(bytes).ToLowerInvariant();
        }

        public static bool VerifyToken(string rawToken, string dbHashedToken)
        {
            var rawHash = SHA256.HashData(Encoding.UTF8.GetBytes(rawToken));
            var storedBytes = Convert.FromHexString(dbHashedToken);
            return CryptographicOperations.FixedTimeEquals(rawHash, storedBytes);
        }
    }
}
