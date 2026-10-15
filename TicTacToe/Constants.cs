using System.Globalization;

namespace TicTacToe
{
    /// <summary>
    /// Stores project-wide constants for easy accessibility and maintainability.
    /// </summary>
    internal static class Constants
    {
        /// <summary>
        /// The date-time format to be used throughout the project.
        /// </summary>
        internal const string DateTimeFormat = "MM/dd/yyyy HH:mm";

        /// <summary>
        /// The culture to be used throughout the project.
        /// </summary>
        internal static readonly CultureInfo ProjectCulture = CultureInfo.InvariantCulture;
    }
}
