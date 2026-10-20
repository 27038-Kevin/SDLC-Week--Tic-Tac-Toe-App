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

        /// <summary>
        /// Gets a line of input from the user.
        /// </summary>
        /// <param name="prompt">The user prompt.</param>
        /// <returns>The line inputted by the user.</returns>
        public static string GetLine(string prompt)
        {
            Console.CursorVisible = true;
            Console.WriteLine(prompt);
            string input = Console.ReadLine()?.Trim() ?? string.Empty;
            Console.CursorVisible = false;
            return input;
        }

        /// <summary>
        /// Prompts confirmation for a sensitive action from the user.
        /// </summary>
        /// <param name="action">The action that needs to be confirmed.</param>
        /// <param name="followup">Follow-up prompt, if required to prompt the user to proceed with caution.</param>
        /// <returns>true if the user confirms the action; false otherwise.</returns>
        public static bool PromptConfirmation(string action, string? followup = default)
        {
            Console.WriteLine();
            Console.WriteLine($"Are you sure you want to {action}?");
            if (followup is not null)
            {
                Console.WriteLine(followup);
                Console.WriteLine();
            }
            Console.WriteLine("Press Y to confirm.");

            var keyPress = GetKeyPress();
            return keyPress.Key == ConsoleKey.Y;
        }

        /// <summary>
        /// Prompts the user to press any key to continue.
        /// </summary>
        public static void PromptContinuation()
        {
            Console.WriteLine("Press any key to continue...");
            GetKeyPress();
        }

        /// <summary>
        /// Displays a message after a process fails multiple times due to invalid user input.
        /// </summary>
        /// <param name="failureMessage">The message to be displayed, informing the user of the failure.</param>
        /// <param name="abortionMessage">The message to be displayed when the process is finally terminated.</param>
        /// <param name="maxTries">The maximum number of allowed tries for the process.</param>
        /// <param name="currentTry">How many times the process has been tried currently.</param>
        internal static void DisplayFailureMessage(string failureMessage, string abortionMessage, int maxTries, int currentTry)
        {
            Console.WriteLine(failureMessage);
            if (maxTries > currentTry)
            {
                int remainingTries = maxTries - currentTry;
                Console.WriteLine($"You have {remainingTries} {(remainingTries == 1 ? "try" : "tries")} left.");
            }
            else
            {
                Console.WriteLine($"Error: Not successful after {maxTries} tries.");
                Console.WriteLine(abortionMessage);
            }

            PromptContinuation();
        }

        /// <summary>
        /// Displays a message after a process succeeds.
        /// </summary>
        /// <param name="message">The message to be displayed, informing the user of the action's success.</param>
        internal static void DisplaySuccessMessage(string message)
        {
            Console.WriteLine(message);
            Console.WriteLine();
            PromptContinuation();
        }
    }
}
