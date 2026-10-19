namespace TicTacToe.Enums
{
    /// <summary>
    /// Provides the context under which credentials are being taken from the user.
    /// </summary>
    internal enum AuthContext
    {
        /// <summary>
        /// Under an authentication context, the password is used for login.
        /// </summary>
        Authentication,

        /// <summary>
        /// Under a confirmation context, the password is re-entered to confirm the user's initial input.
        /// </summary>
        Confirmation,

        /// <summary>
        /// Under a creation context, a new password is being taken to be saved at the time of registration.
        /// </summary>
        Creation,

        /// <summary>
        /// Under a reset context, a new password is being taken to replace an old password.
        /// </summary>
        Reset,
    }
}