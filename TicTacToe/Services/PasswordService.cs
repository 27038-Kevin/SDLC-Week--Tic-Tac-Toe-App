using System.Collections.Immutable;
using System.Security.Cryptography;
using System.Text;
using TicTacToe.Models.Data;

namespace TicTacToe.Services
{
    /// <summary>
    /// Manages the handling of passwords throughout the application.
    /// </summary>
    public static class PasswordService
    {
        private const int KeySize = Constants.PasswordKeySize;
        private const int Iterations = Constants.PasswordHashIterations;
        private static readonly HashAlgorithmName HashAlgorithm = HashAlgorithmName.SHA256;

        /// <summary>
        /// Hashes a password string.
        /// </summary>
        /// <param name="password">The password string to hash.</param>
        /// <returns>The password's hash value.</returns>
        public static Password HashPassword(string password)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(KeySize);

            byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
                            Encoding.UTF8.GetBytes(password),
                            salt,
                            Iterations,
                            HashAlgorithm,
                            KeySize);

            return new Password
            {
                Hashcode = Convert.ToHexString(hash),
                Salt = salt.ToImmutableArray(),
            };
        }

        /// <summary>
        /// Verifies a user-inputted password against the salted and hashed password stored in the repository.
        /// </summary>
        /// <param name="input">The user-inputted password to be verified.</param>
        /// <param name="originalPassword">The original password stored in the repository.</param>
        /// <returns>Whether or not the entered password matches the original password.</returns>
        public static bool VerifyPassword(string input, Password originalPassword)
        {
            byte[] salt = originalPassword.Salt
                                .ToArray();

            byte[] hashToCompare = Rfc2898DeriveBytes.Pbkdf2(
                    input, salt, Iterations, HashAlgorithm, KeySize);

            return CryptographicOperations
                    .FixedTimeEquals(hashToCompare, Convert.FromHexString(originalPassword.Hashcode));
        }
    }
}