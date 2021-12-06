using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Electronic_Organizer_API.Security
{
    public class PasswordHashing
    {
        public string ToHexString(byte[] hex)
        {
            if (hex == null) return null;
            if (hex.Length == 0) return string.Empty;

            var s = new StringBuilder();
            foreach (byte b in hex)
            {
                s.Append(b.ToString("x2"));
            }
            return s.ToString();
        }
        public string CreateSalt()
        {
            RNGCryptoServiceProvider rng = new();
            byte[] buff = new byte[32];
            rng.GetBytes(buff);
            return Convert.ToBase64String(buff);
        }
        public string GenerateHash(string input, string salt)
        {
            SHA256 sha256 = SHA256.Create();
            byte[] bytes = Encoding.UTF8.GetBytes(input + salt);
            byte[] hash = sha256.ComputeHash(bytes);
            return ToHexString(hash);
        }

        public bool IsPassowrdValid(string passwordToValidate, string salt, string correctPasswordHash)
        {
            string newHashedPin = GenerateHash(passwordToValidate, salt);
            return newHashedPin.SequenceEqual(correctPasswordHash);
        }
    }
}
