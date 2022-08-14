using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;
using System.Text;

namespace auth_web_api.Repositories.Passwordhashers.HmasShaRepository
{
    public class HMACSHA256Repository : IPasswordHasher
    {
        public string Hash(string password)
        {
            const string SALT = "CGYzqeN4plZekNC88Umm1Q==";

            string hashedPassword = Convert.ToBase64String(KeyDerivation.Pbkdf2(
               password: password,
               salt: Encoding.UTF8.GetBytes(SALT),
               prf: KeyDerivationPrf.HMACSHA256,
               iterationCount: 100000,
               numBytesRequested: 256 / 8));

            return hashedPassword;
        }

        public bool VerifyPassword(string givenPassword, string actualPasswordHash)
        {
            string hashOfGivenPassword = Hash(givenPassword);

            if (actualPasswordHash == hashOfGivenPassword)
            {
                return true;
            }

            return false;
        }
    }
}
