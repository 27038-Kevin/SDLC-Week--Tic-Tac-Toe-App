namespace TicTacToe.Exceptions
{
    /// <summary>
    /// Exception thrown when a menu is cancelled by the user.
    /// </summary>
    internal class MenuCancelledException : ApplicationException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MenuCancelledException"/> class.
        /// </summary>
        public MenuCancelledException()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MenuCancelledException"/> class.
        /// </summary>
        /// <param name="message">The message to be passed on to the exception.</param>
        public MenuCancelledException(string message)
            : base(message)
        {
        }
    }
}
