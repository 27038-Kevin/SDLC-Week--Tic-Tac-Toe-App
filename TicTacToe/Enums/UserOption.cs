namespace TicTacToe.Enums
{
    /// <summary>
    /// Menu options for the user to perform on their account.
    /// </summary>
    internal enum UserOption
    {
        /// <summary>
        /// Allows the user to change their name.
        /// </summary>
        ChangeName,

        /// <summary>
        /// Allows the user to reset their password.
        /// </summary>
        ResetPassword,

        /// <summary>
        /// Allows the user to wipe all their data.
        /// </summary>
        FactoryReset,

        /// <summary>
        /// Allows the user to delete their account.
        /// </summary>
        DeleteAccount,
    }
}
