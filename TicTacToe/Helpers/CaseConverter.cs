using System.Text;

namespace TicTacToe.Helpers
{
    /// <summary>
    /// Converts different types of cases (camelCase, PascalCase, kebab-case, snake_case, Title Case, etc.)
    /// </summary>
    public static class CaseConverter
    {
        /// <summary>
        /// Converts a PascalCase string to a normal Title Cased string.
        /// </summary>
        /// <param name="pascalCasedName">The PascalCased string.</param>
        /// <returns>Title-Cased string.</returns>
        public static string PascalToTitle(string pascalCasedName)
        {
            var titleCasedName = new StringBuilder();

            for (int i = 0; i < pascalCasedName.Length; ++i)
            {
                if (char.IsUpper(pascalCasedName[i]) && i != 0)
                {
                    titleCasedName.Append(' ');
                }

                titleCasedName.Append(pascalCasedName[i]);
            }

            return titleCasedName.ToString();
        }

        /// <summary>
        /// Converts a rAndOm-caSeD string to a normal Title Cased string.
        /// </summary>
        /// <param name="randomCasedName">The RAndoM-cAseD string.</param>
        /// <returns>Title-Cased string.</returns>
        public static string RandomToTitle(string randomCasedName)
        {
            var titleCasedName = new StringBuilder();

            for (int i = 0; i < randomCasedName.Length; ++i)
            {
                if (titleCasedName.Length == 0 || char.IsWhiteSpace(titleCasedName[i - 1]))
                {
                    // If first character, or last character was a space, then append an uppercase character
                    titleCasedName.Append(
                        char.ToUpper(randomCasedName[i]));
                }
                else
                {
                    // Otherwise, append a lowercase character
                    titleCasedName.Append(
                        char.ToLower(randomCasedName[i]));
                }
            }

            return titleCasedName.ToString();
        }
    }
}
