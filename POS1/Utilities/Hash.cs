using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;


namespace POS1.Utilities
{
    public class Hash
    {

        public static string HashPassword(string password)
        {
            // Generate a random salt
            byte[] salt = new byte[128 / 8]; // 128 bits
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            // Hash the password using PBKDF2
            string hashedPassword = Convert.ToBase64String(
                KeyDerivation.Pbkdf2(
                    password: password,
                    salt: salt,
                    prf: KeyDerivationPrf.HMACSHA256,
                    iterationCount: 10000, // number of iterations
                    numBytesRequested: 256 / 8 // 256 bits
                )
            );

            // Combine the salt and hashed password
            string combinedHash = $"{Convert.ToBase64String(salt)}:{hashedPassword}";

            return combinedHash;
        }

        public static bool VerifyPassword(string password, string hashedPassword)
        {
            // Extract the salt and hashed password from the stored hash
            string[] parts = hashedPassword.Split(':');
            byte[] salt = Convert.FromBase64String(parts[0]);
            string storedHashedPassword = parts[1];

            // Compute the hash of the provided password and verify it against the stored hash
            string computedHash = Convert.ToBase64String(
                KeyDerivation.Pbkdf2(
                    password: password,
                    salt: salt,
                    prf: KeyDerivationPrf.HMACSHA256,
                    iterationCount: 10000, // number of iterations
                    numBytesRequested: 256 / 8 // 256 bits
                )
            );

            return storedHashedPassword == computedHash;
        }
       

    }

}


