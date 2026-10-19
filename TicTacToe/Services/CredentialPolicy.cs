using System.Collections.Immutable;

namespace TicTacToe.Services
{
    /// <summary>
    /// Handles authentication/authorization throughout the application.
    /// </summary>
    internal static class CredentialPolicy
    {
        /// <summary>
        /// Defines the minimum length of a password.
        /// </summary>
        internal const int MinimumPasswordLength = 8;

        /// <summary>
        /// Defines the maximum length of a password.
        /// </summary>
        internal const int MaximumPasswordLength = 50;

        /// <summary>
        /// Defines the minimum length of a username.
        /// </summary>
        internal const int MinimumUsernameLength = 5;

        /// <summary>
        /// Defines the maximum length of a username.
        /// </summary>
        internal const int MaximumUsernameLength = 30;

        /// <summary>
        /// Defines the password policies used for password creation and reset.
        /// </summary>
        internal static readonly ImmutableDictionary<string, Predicate<string>> PasswordPolicies = new Dictionary<string, Predicate<string>>()
        {
            // The password should not be null or empty.
            ["Non-Empty Password Policy"] = (plaintextPassword) => plaintextPassword is not null
                                                                && plaintextPassword != string.Empty,

            // The password must not contain fewer characters than the permitted minimum length.
            // The password must not exceed the permitted maximum length as this would make it prone to being forgotten.
            ["Password Length Policy"] = (plaintextPassword) => plaintextPassword.Length >= MinimumPasswordLength
                                                                && plaintextPassword.Length <= MaximumPasswordLength,

            // The password must contain at least one uppercase letter.
            // The password must contain at least one lowercase letter.
            // The password must contain at least a single digit.
            // The password should contain at least a single symbol, whitespace, or punctuation mark.
            ["Character Combination Policy"] = (plaintextPassword) => plaintextPassword.Any(char.IsUpper)
                                                                    && plaintextPassword.Any(char.IsLower)
                                                                    && plaintextPassword.Any(char.IsDigit)
                                                                    && !plaintextPassword.All(char.IsLetterOrDigit),
        }
        .ToImmutableDictionary();

        /// <summary>
        /// Defines the username policies used for creation of a username during registration.
        /// </summary>
        internal static readonly ImmutableDictionary<string, Predicate<string>> UsernamePolicies = new Dictionary<string, Predicate<string>>()
        {
            // The username should not be null or empty.
            ["Non-Empty Username Policy"] = (username) => username is not null
                                                        && username != string.Empty,

            // The username should not contain less than the permitted minimum length.
            // The username should also not exceed the permitted maximum length.
            ["Username Length Policy"] = (username) => username.Length >= MinimumUsernameLength
                                                    && username.Length <= MaximumUsernameLength,

            // The username should not contain symbols, punctuation marks or whitespaces.
            // The username should have at least one letter.
            ["Valid Characters Policy"] = (username) => username.All(char.IsLetterOrDigit)
                                                        && username.Any(char.IsLetter),
        }
        .ToImmutableDictionary();

        /// <summary>
        /// Validates a given password against pre-defined password rules.
        /// </summary>
        /// <param name="plaintextPassword">The password to validate.</param>
        /// <returns>true if the password follows all the business rules; false otherwise.</returns>
        internal static bool ValidatePasswordRules(string plaintextPassword)
        {
            return PasswordPolicies.Values
                    .All(policy => plaintextPassword.Satisfies(policy));
        }

        /// <summary>
        /// Validates a given username against pre-defined username rules.
        /// </summary>
        /// <param name="username">The username to validate.</param>
        /// <returns>true if the username follows all the business rules; false otherwise.</returns>
        internal static bool ValidateUsernameRules(string username)
        {
            username = username.Trim();

            return UsernamePolicies.Values
                    .All(policy => username.Satisfies(policy));
        }

        /// <summary>
        /// Helper method to determine whether a given string satisfies some policy.
        /// </summary>
        /// <param name="text">The string to test.</param>
        /// <param name="policy">The policy to validate.</param>
        /// <returns>true if the string satisfies the specified policy; false otherwise.</returns>
        private static bool Satisfies(this string text, Predicate<string> policy)
        {
            return policy(text);
        }
    }
}
