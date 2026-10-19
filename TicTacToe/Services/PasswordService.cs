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
        /// <param name="salt">The salt used to hash the password. If not included, one is generated.</param>
        /// <returns>The password's hash value.</returns>
        public static Password HashPassword(string password, byte[]? salt = default)
        {
            salt ??= RandomNumberGenerator.GetBytes(KeySize);

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
        /// Verifies a hash of a user-inputted password against the hashed password stored in the repository.
        /// </summary>
        /// <param name="inputtedPassword">The hash of the user-inputted password to be verified.</param>
        /// <param name="originalPassword">The hash of the original password stored in the repository.</param>
        /// <returns>Whether or not the entered password matches the original password.</returns>
        public static bool VerifyPassword(Password inputtedPassword, Password originalPassword)
        {
            byte[] inputtedPasswordHash = Convert.FromHexString(inputtedPassword.Hashcode);
            byte[] originalPasswordHash = Convert.FromHexString(originalPassword.Hashcode);

            return CryptographicOperations
                    .FixedTimeEquals(inputtedPasswordHash, originalPasswordHash);
        }
    }
}