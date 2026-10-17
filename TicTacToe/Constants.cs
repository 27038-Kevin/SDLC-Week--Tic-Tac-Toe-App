using System.Globalization;

namespace TicTacToe
{
    /// <summary>
    /// Stores project-wide constants for easy accessibility and maintainability.
    /// </summary>
    internal static class Constants
    {
        /// <summary>
        /// The date format to be used throughout the project. Used where time is not required.
        /// </summary>
        internal const string DateFormat = "MM/dd/yyyy";

        /// <summary>
        /// The date-time format to be used throughout the project.
        /// </summary>
        internal const string DateTimeFormat = "MM/dd/yyyy HH:mm";

        /// <summary>
        /// The name of the directory where persistent data resides.
        /// </summary>
        internal const string DataDirectoryName = "data";

        /// <summary>
        /// The CSV file where the game summaries are stored.
        /// </summary>
        internal const string GamesFileName = "games.csv";

        /// <summary>
        /// Temporarily-used name for demonstration.
        /// </summary>
        internal const string TempName = "Kevin";

        /// <summary>
        /// The culture to be used throughout the project.
        /// </summary>
        internal static readonly CultureInfo ProjectCulture = CultureInfo.InvariantCulture;

        /// <summary>
        /// Gets the greeting to the user, based on the time of the day.
        /// </summary>
        /// <value>
        /// "Good Evening" if the time is at night or beyond 5 PM.
        /// "Good Morning" if the time is between 3 AM and 12 PM.
        /// "Good Afternoon" if the time is between 12 PM and 5 PM.
        /// </value>
        /// <remarks>"Good Night" is not used as a greeting as it is a parting phrase.</remarks>
        internal static string Greeting => DateTime.Now.Hour switch
        {
            < 3 => "Good Evening",
            < 12 => "Good Morning",
            < 17 => "Good Afternoon",
            < 24 => "Good Evening",
            _ => throw new ArgumentException("Invalid hour"),
        };
    }
}
