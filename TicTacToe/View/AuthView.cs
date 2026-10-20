using System.ComponentModel;
using System.Text;
using TicTacToe.Enums;
using TicTacToe.Models.Data;
using TicTacToe.Models.StateMachines;
using TicTacToe.Services;

namespace TicTacToe.View
{
    /// <summary>
    /// View renders for authentication mechanisms.
    /// </summary>
    internal static class AuthView
    {
        private const string DoubleLine = @"

";

        private const string AbortLoginMessage = "Login failed. Please try again later.";
        private const string AbortPasswordConfirmationMessage = "The password was not confirmed; the requested action could not be processed.";
        private const string AbortPasswordResetMessage = "Password could not be reset.";
        private const string InvalidPasswordConfirmationMessage = "Wrong password! Your password has not been confirmed.";
        private const string LoginFailureMessage = "Wrong username or password.";
        private const string NewPasswordSameAsPreviousMessage = "Your new password cannot be the same as your previous password!";

        private const string PasswordRulesViolationMessage =
@"Your password must contain 8+ characters with at least one lowercase,
one uppercase, one digit, one symbol, and not more than 50 characters.";

        private const string UserAlreadyExistsMessage =
@"This user already exists!
If this is you, try logging in instead.";

        private const string UsernameRulesViolationMessage =
@"Your username should have 5-30 characters, should have at least a single letter,
and must not contain any symbols, whitespaces or punctuation marks.";

        /// <summary>
        /// Displays a failure message for user's invalid password inputs based on the context.
        /// </summary>
        /// <param name="context">The context under which the password is taken as input.</param>
        /// <param name="maximumAllowedTries">The maximum number of allowed input tries.</param>
        /// <param name="currentTry">The current try the user is on.</param>
        internal static void DisplayBadPasswordMessage(
                            AuthContext context,
                            int maximumAllowedTries,
                            int currentTry)
        {
            switch (context)
            {
                case AuthContext.Authentication:
                    ConsoleUtils.DisplayFailureMessage(
                            LoginFailureMessage,
                            AbortLoginMessage,
                            maximumAllowedTries,
                            currentTry);
                    break;

                case AuthContext.Confirmation:
                    ConsoleUtils.DisplayFailureMessage(
                            InvalidPasswordConfirmationMessage,
                            AbortPasswordConfirmationMessage,
                            maximumAllowedTries,
                            currentTry);
                    break;

                case AuthContext.Creation:
                    Console.WriteLine(PasswordRulesViolationMessage);
                    Console.WriteLine();
                    ConsoleUtils.PromptContinuation();
                    break;

                case AuthContext.Reset:
                    ConsoleUtils.DisplayFailureMessage(
                            NewPasswordSameAsPreviousMessage,
                            AbortPasswordResetMessage,
                            maximumAllowedTries,
                            currentTry);
                    break;
            }
        }

        /// <summary>
        /// Displays a message when the typed username in a Creation context
        /// either already exists, or does not follow the username policy.
        /// </summary>
        /// <param name="doesUserAlreadyExist">Flag for whether the user already exists.</param>
        /// <param name="areUsernameRulesNotFollowed">Flag for whether the username policy is followed.</param>
        /// <exception cref="ArgumentException">Thrown if both flags are false; then the username would be acceptable.</exception>
        internal static void DisplayBadUsernameMessage(
                    bool doesUserAlreadyExist = false,
                    bool areUsernameRulesNotFollowed = false)
        {
            if (!doesUserAlreadyExist && !areUsernameRulesNotFollowed)
            {
                throw new ArgumentException("Cannot display bad messages for good input");
            }

            if (doesUserAlreadyExist)
            {
                Console.WriteLine(UserAlreadyExistsMessage);
                Console.WriteLine();
                ConsoleUtils.PromptContinuation();
            }
            else if (areUsernameRulesNotFollowed)
            {
                Console.WriteLine(UsernameRulesViolationMessage);
                Console.WriteLine();
                ConsoleUtils.PromptContinuation();
            }
        }

        /// <summary>
        /// Gets a password from the user and returns its hash.
        /// The plaintext password is never exposed indeliberately; the user can toggle its visibility on the screen.
        /// </summary>
        /// <param name="context">The context under which the password is being taken from the user as input.</param>
        /// <param name="produceHash">A method abstraction to produce the hash without ever directly exposing the plaintext.</param>
        /// <param name="rulesCheck">A policy/rule to be checked; if included, the result must be updated.</param>
        /// <returns>The password's hashcode, and the salt used to hash the password.</returns>
        internal static Password GetPassword(
                    AuthContext context,
                    Func<string, Password> produceHash,
                    PolicyMarshal<string>? rulesCheck = default)
        {
            string viewDisplayString = GetPasswordDisplayString(context);
            string contextInstructions = GetContextInstructions(
                    context,
                    DynamicStringBuilder.ToggleKey,
                    DynamicStringBuilder.SubmitKey);

            Password password = default;
            EventHandler<StringBuilder> savePasswordOnSubmit = (obj, passwordStringBuilder) =>
            {
                password = produceHash(passwordStringBuilder.ToString());
                if (rulesCheck is not null)
                {
                    rulesCheck.ComplianceStatus = rulesCheck.Policy(
                                                    passwordStringBuilder.ToString())
                                                ? PolicyCompliance.Compliant
                                                : PolicyCompliance.Incompliant;
                }
            };

            var passwordBuilder = new DynamicStringBuilder(
                    isSecret: true,
                    enableVisibilityToggle: GetEnableVisibilityToggle(context),
                    savePasswordOnSubmit);

            while (!passwordBuilder.IsSubmitted)
            {
                ConsoleUtils.ClearScreen();
                Console.WriteLine(
                    string.Join(
                        separator: DoubleLine,
                        viewDisplayString,
                        passwordBuilder.DisplayString + '_',    // Underscore to highlight imaginary cursor position
                        contextInstructions));

                ConsoleKeyInfo keyPress = ConsoleUtils.GetKeyPress();
                passwordBuilder.UpdateState(keyPress);
            }

            ConsoleUtils.ClearScreen(); // Clear console to hide potentially visible plaintext password from sight
            return password;
        }

        /// <summary>
        /// Gets a username from the user and returns it.
        /// </summary>
        /// <param name="context">The context under which the username is being taken from the user as input.</param>
        /// <returns>The username, trimmed and cleaned.</returns>
        internal static string GetUsername(AuthContext context)
        {
            string viewDisplayString = GetUsernameDisplayString(context);
            string username = default!;
            EventHandler<StringBuilder> saveUsernameOnSubmit = (obj, usernameStringBuilder) =>
            {
                username = usernameStringBuilder.ToString().Trim();
            };

            var usernameBuilder = new DynamicStringBuilder(
                    isSecret: false,
                    enableVisibilityToggle: false,
                    saveUsernameOnSubmit);

            while (!usernameBuilder.IsSubmitted)
            {
                ConsoleUtils.ClearScreen();
                Console.WriteLine(
                    string.Join(
                        separator: DoubleLine,
                        viewDisplayString,
                        usernameBuilder.DisplayString + '_'));  // Underscore to highlight imaginary cursor position

                ConsoleKeyInfo keyPress = ConsoleUtils.GetKeyPress();
                usernameBuilder.UpdateState(keyPress);
            }

            return username;
        }

        /// <summary>
        /// Gets instructions for the user to enter the password after displaying the view.
        /// </summary>
        /// <param name="context">The context under which the password is taken as input.</param>
        /// <param name="toggleKey">The key used to toggle password visibility.</param>
        /// <param name="submitKey">The key used to submit the password.</param>
        /// <returns>Message containing the instructions to be displayed to the user.</returns>
        /// <exception cref="InvalidEnumArgumentException">Thrown if the context is unknown.</exception>
        private static string GetContextInstructions(AuthContext context, ConsoleKey toggleKey, ConsoleKey submitKey)
        {
            string generalInstructions =
@$"Press {toggleKey} to toggle password visibility,
or press {submitKey} to submit the password.

Use Backspace to undo typing the last character.";

            return context switch
            {
                AuthContext.Authentication or AuthContext.Reset => generalInstructions,
                AuthContext.Confirmation => string.Empty,
                AuthContext.Creation => generalInstructions + $"{Environment.NewLine}You can use letters, numbers, symbols, and spaces in the password.",
                _ => throw new InvalidEnumArgumentException(nameof(context)),
            };
        }

        /// <summary>
        /// Gets the string to be displayed when taking password input with respect to the context.
        /// </summary>
        /// <param name="context">The context in which the password is taken from the user.</param>
        /// <returns>The appropriate display string.</returns>
        /// <exception cref="InvalidEnumArgumentException">Thrown if the context is unknown.</exception>
        private static string GetPasswordDisplayString(AuthContext context)
        {
            return context switch
            {
                AuthContext.Authentication => "Enter your password.",

                AuthContext.Confirmation => "Re-enter your password to confirm.",

                AuthContext.Creation or AuthContext.Reset => @$"Enter your new password.
A strong password usually includes 8+ characters with at least a single lowercase, uppercase, numeral and symbol.",

                _ => throw new InvalidEnumArgumentException(nameof(context)),
            };
        }

        /// <summary>
        /// Gets the string to be displayed when taking username input with respect to the context.
        /// </summary>
        /// <param name="context">The authentication/authorization context under which the username is taken from the user.</param>
        /// <returns>The appropriate display string.</returns>
        /// <exception cref="NotSupportedException">
        /// Thrown if the username is taken in a Confirmation context; there is no reason to take the username here.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown if the username is taken in a Reset context; the username cannot be modified.
        /// </exception>
        /// <exception cref="InvalidEnumArgumentException">Thrown if the context is unrecognized/unknown.</exception>
        private static string GetUsernameDisplayString(AuthContext context)
        {
            return context switch
            {
                AuthContext.Authentication => "Enter your username.",
                AuthContext.Creation => $@"Enter your username.
Your username must have between {CredentialPolicy.MinimumUsernameLength} and {CredentialPolicy.MaximumUsernameLength} characters, and should not contain any symbols, spaces or punctuation marks.
Choose your username wisely; it cannot be changed.",

                AuthContext.Confirmation => throw new NotSupportedException("No reason to confirm username"),
                AuthContext.Reset => throw new InvalidOperationException("Cannot reset username"),
                _ => throw new InvalidEnumArgumentException(nameof(context)),
            };
        }

        /// <summary>
        /// Gets whether or not to toggle visibility for the password input.
        /// </summary>
        /// <param name="context">The context under which the password is taken from the user.</param>
        /// <returns>true if the visibility toggle can be enabled; false otherwise.</returns>
        /// <exception cref="InvalidEnumArgumentException">Thrown if the context is unknown.</exception>
        private static bool GetEnableVisibilityToggle(AuthContext context)
        {
            return context switch
            {
                AuthContext.Authentication
                    or AuthContext.Creation
                    or AuthContext.Reset => true,

                AuthContext.Confirmation => false,

                _ => throw new InvalidEnumArgumentException(nameof(context)),
            };
        }
    }
}
