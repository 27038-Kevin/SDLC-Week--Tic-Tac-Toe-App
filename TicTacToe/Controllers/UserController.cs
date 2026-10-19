using TicTacToe.Enums;
using TicTacToe.Models.Data;
using TicTacToe.Models.StateMachines;
using TicTacToe.Services;
using TicTacToe.View;

namespace TicTacToe.Controllers
{
    internal class UserController
    {
        private const int MaximumAllowedTries = Constants.MaximumAllowedInputTries;

        private readonly UserService _service;
        private readonly string _dataDirectory;

        public UserController(UserService service, string dataDirectory)
        {
            this._service = service;
            this._dataDirectory = dataDirectory;
        }

        internal event EventHandler<string>? OnUserNameChange;

        public User? CurrentUser { get; private set; } = default;

        public bool IsSessionActive => this.CurrentUser is not null;

        /// <summary>
        /// Closes a user session and restores the active user ID to its default value.
        /// </summary>
        /// <param name="implicitClosure">
        /// Depicts whether to take user confirmation before closing the session.
        /// If true, session is closed automatically.
        /// </param>
        internal void CloseUserSession(bool implicitClosure = false)
        {
            if (!implicitClosure)
            {
                bool confirmation = ConsoleUtils.PromptConfirmation("log out");
                if (!confirmation)
                {
                    return;
                }
            }

            this.CurrentUser = default;
        }

        /// <summary>
        /// Gets various operations the user can perform directly on their account.
        /// </summary>
        /// <exception cref="FieldAccessException">Thrown if there is an attempt to access the menu without initiating a session.</exception>
        internal void GetUserOptions()
        {
            if (!this.IsSessionActive)
            {
                throw new FieldAccessException("Cannot access user ID when user session is not yet initiated");
            }

            var userMenu = new Menu<UserOption>();

            string menuHeader = "USER ACTIONS";
            string menuFooter = Menu.GetBasicMenuFooter(isMenuCancellable: true);

            foreach (UserOption selection in userMenu.RunPersistently(menuHeader, menuFooter, isMenuCancellable: true))
            {
                switch (selection)
                {
                    case UserOption.ChangeName:
                        this.ChangeName();
                        break;

                    case UserOption.ResetPassword:
                        this.ResetPassword();
                        break;

                    case UserOption.FactoryReset:
                        this.ResetUserData();
                        break;

                    case UserOption.DeleteAccount:
                        this.DeleteAccount();
                        break;
                }

                if (!this.IsSessionActive)
                {
                    break;
                }
            }
        }

        /// <summary>
        /// Logs a user into the application by taking credentials and authenticating the user.
        /// If authentication is successful, a user session is intiated.
        /// </summary>
        internal void LoginUser()
        {
            User? user = default;

            for (int tryCount = 1; tryCount <= MaximumAllowedTries; ++tryCount)
            {
                string username = ConsoleUtils.GetLine("Enter your username:");
                bool usernameExists = this._service.DoesUsernameExist(username);
                if (usernameExists)
                {
                    user = this._service.GetUser(username);
                }

                Password password = AuthView.GetPassword(
                                    AuthContext.Authentication,
                                    HashInputPassword);

                bool authenticated = user is not null
                        && PasswordService.VerifyPassword(
                            password, user.Password);

                if (authenticated)
                {
                    this.ActivateUserSession(user);
                    break;
                }
                else
                {
                    AuthView.DisplayBadPasswordMessage(
                        AuthContext.Authentication,
                        MaximumAllowedTries,
                        tryCount);
                }
            }

            Password HashInputPassword(string input)
            {
                /**
                 * To retrieve the original salt, the user must have been found.
                 * If the username does not exist, then the password will be discarded, regardless of the salt used.
                 * Because of this, it is more analogous to use the default value.
                 */
                if (user is null)
                {
                    return default;
                }

                byte[] originalSalt = user.Password.Salt.ToArray();
                return PasswordService.HashPassword(input, originalSalt);
            }
        }

        /// <summary>
        /// Registers a user to the repository.
        /// </summary>
        internal void RegisterUser()
        {
            string name = ConsoleUtils.GetLine("Enter your name:");
            string username = default!;
            Password password = default;

            bool gotUsername = false;
            bool gotPassword = false;

            while (!gotUsername)
            {
                username = AuthView.GetUsername(AuthContext.Creation);
                bool userExists = this._service.DoesUsernameExist(username);
                if (userExists)
                {
                    AuthView.DisplayBadUsernameMessage(doesUserAlreadyExist: true);
                    continue;
                }

                bool usernameRulesFollowed = CredentialPolicy.ValidateUsernameRules(username);
                if (!usernameRulesFollowed)
                {
                    AuthView.DisplayBadUsernameMessage(areUsernameRulesNotFollowed: true);
                    continue;
                }

                gotUsername = true;
            }

            while (!gotPassword)
            {
                var passwordRulesCheck = new PolicyMarshal<string>(CredentialPolicy.ValidatePasswordRules);

                password = AuthView.GetPassword(
                            AuthContext.Creation,
                            (input) => PasswordService.HashPassword(input),
                            passwordRulesCheck);

                bool passwordRulesFollowed =
                    passwordRulesCheck.ComplianceStatus == PolicyCompliance.Compliant;

                if (!passwordRulesFollowed)
                {
                    AuthView.DisplayBadPasswordMessage(AuthContext.Creation, MaximumAllowedTries, default);
                    continue;
                }

                gotPassword = true;
            }

            bool confirmed = this.ConfirmPassword(password);
            if (confirmed)
            {
                this._service.RegisterUser(username, password, name);
                ConsoleUtils.DisplaySuccessMessage("Registered successfully!");
            }
        }

        /// <summary>
        /// Allows the user to reset their password.
        /// </summary>
        /// <param name="authorized">
        /// Whether or not the system is authorized to reset the user's password.
        /// true if the user triggered an account recovery option.
        /// </param>
        internal void ResetPassword(bool authorized = false)
        {
            if (!authorized)
            {
                bool confirmation = ConsoleUtils.PromptConfirmation("change your password?");
                if (!confirmation)
                {
                    return;
                }
            }

            Password newPassword = default;
            bool newPasswordAcquired = false;
            for (int tryCount = 1; tryCount <= MaximumAllowedTries && !newPasswordAcquired; ++tryCount)
            {
                var policyMarshal = new PolicyMarshal<string>(CredentialPolicy.ValidatePasswordRules);
                bool isPasswordSame = default;

                newPassword = AuthView.GetPassword(
                                AuthContext.Reset,
                                (input) =>
                                {
                                    Password hashToCompare = PasswordService.HashPassword(
                                            input, this.CurrentUser!.Password.Salt.ToArray());
                                    isPasswordSame = PasswordService.VerifyPassword(hashToCompare, this.CurrentUser.Password);

                                    // If the passwords are the same, the hashed value will be discarded regardless
                                    if (isPasswordSame)
                                    {
                                        return default;
                                    }

                                    newPasswordAcquired = true;
                                    return PasswordService.HashPassword(input); // Provide fresh hash with new salt
                                },
                                policyMarshal);

                if (isPasswordSame)
                {
                    AuthView.DisplayBadPasswordMessage(
                        AuthContext.Reset,
                        MaximumAllowedTries,
                        tryCount);
                }
            }

            if (!newPasswordAcquired)
            {
                return;
            }

            bool passwordResetConfirmed = this.ConfirmPassword(newPassword);
            if (passwordResetConfirmed)
            {
                this.CurrentUser!.Password = newPassword;
                this._service.UpdateUser(this.CurrentUser);
                ConsoleUtils.DisplaySuccessMessage("Password reset successfully!");
            }
        }

        /// <summary>
        /// Initiates a new user session and saves the user's details for availing user services.
        /// </summary>
        /// <param name="user">The user.</param>
        private void ActivateUserSession(User? user)
        {
            this.CurrentUser = user;
        }

        /// <summary>
        /// Changes the user's name.
        /// </summary>
        private void ChangeName()
        {
            if (this.CurrentUser is null)
            {
                return;
            }

            this.CurrentUser.Name = ConsoleUtils.GetLine("Enter your new name:");
            this._service.UpdateUser(this.CurrentUser);
            ConsoleUtils.DisplaySuccessMessage($"Your name has been changed to {this.CurrentUser?.Name}!");
        }

        /// <summary>
        /// Gets user confirmation by prompting the user to retype the previously entered password.
        /// </summary>
        /// <param name="password">The password to be confirmed.</param>
        /// <returns>true if the user retypes the correct password, false if the user fails to retype the correct password after the maximum allowed tries.</returns>
        private bool ConfirmPassword(Password password)
        {
            for (int tryCount = 1; tryCount <= MaximumAllowedTries; ++tryCount)
            {
                Password retypedPassword = AuthView.GetPassword(
                                            AuthContext.Confirmation,
                                            (input) => PasswordService.HashPassword(input));

                bool confirmed = PasswordService.VerifyPassword(retypedPassword, password);
                if (confirmed)
                {
                    return true;
                }
                else
                {
                    AuthView.DisplayBadPasswordMessage(
                        AuthContext.Confirmation,
                        MaximumAllowedTries,
                        tryCount);
                }
            }

            return false;
        }

        /// <summary>
        /// Closes the user's session and deletes their account after taking dual confirmation,
        /// once via keypress and another time using password confirmation.
        /// </summary>
        private void DeleteAccount()
        {
            bool inputConfirmation = ConsoleUtils.PromptConfirmation(
                                    "delete your account",
                                    "WARNING: THIS ACTION IS PERMANENT AND CANNOT BE UNDONE.");
            if (!inputConfirmation)
            {
                return;
            }

            bool passwordConfirmation = this.ConfirmPassword(this.CurrentUser!.Password);
            if (!passwordConfirmation)
            {
                return;
            }

            var userId = this.CurrentUser.Id;
            this.CloseUserSession(implicitClosure: true);
            this._service.DeleteUser(userId);
            DataClearanceService.DeleteUserData(this._dataDirectory, userId, isUserDeleted: true);

            ConsoleUtils.DisplaySuccessMessage(
@"Your data has been cleared and your account has been deleted successfully.
Thank you for playing with us!");
        }

        /// <summary>
        /// Resets user's data.
        /// </summary>
        private void ResetUserData()
        {
            bool confirmation = ConsoleUtils.PromptConfirmation(
                                "clear your data",
                                "This action is permanent and cannot be undone.");
            if (confirmation)
            {
                DataClearanceService.DeleteUserData(this._dataDirectory, this.CurrentUser!.Id);
                ConsoleUtils.DisplaySuccessMessage("Your data has been wiped and your account reset.");
            }
        }
    }
}
