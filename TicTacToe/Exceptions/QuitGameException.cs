namespace TicTacToe.Exceptions
{
    /// <summary>
    /// Exception to be thrown when the user quits a game.
    /// </summary>
    internal sealed class QuitGameException : ApplicationException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QuitGameException"/> class.
        /// </summary>
        internal QuitGameException()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QuitGameException"/> class.
        /// </summary>
        /// <param name="message">The message to be passed on to the exception.</param>
        internal QuitGameException(string message)
            : base(message)
        {
        }
    }
}
