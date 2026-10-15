namespace TicTacToe.View
{
    /// <summary>
    /// Contains utility helper methods for console operations.
    /// </summary>
    internal class ConsoleUtils
    {
        /// <summary>
        /// Clears the entire console screen, including the scrollable history,
        /// by writing an ANSI sequence to the console.
        /// </summary>
        public static void ClearScreen()
        {
            const string clearScreenAnsiSequence = "\u001b[2J";
            const string clearScrollbackAnsiSequence = "\u001b[3J";
            const string moveCursorToTopLeftAnsiSequence = "\u001b[H";

            Console.Write(
                clearScreenAnsiSequence +
                clearScrollbackAnsiSequence +
                moveCursorToTopLeftAnsiSequence);
        }

        /// <summary>
        /// Gets a keypress from the user.
        /// </summary>
        /// <returns>The key pressed by the user.</returns>
        public static ConsoleKeyInfo GetKeyPress()
        {
            return Console.ReadKey(intercept: true);
        }
    }
}
