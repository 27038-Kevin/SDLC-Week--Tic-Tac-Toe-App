namespace TicTacToe.Helpers
{
    /// <summary>
    /// Helps adapt method parameters to more favorable types.
    /// </summary>
    internal static class ParamAdapter
    {
        /// <summary>
        /// Gets a converter which converts a normal string array to a semantically equivalent ranked array.
        /// </summary>
        /// <value>The method that will add a rank to the string array when invoked.</value>
        public static Converter<string[], string[,]> ToRankedArray { get; } = (array) =>
        {
            string[,] rankedArray = new string[array.Length, 1];
            for (int i = 0; i < array.Length; ++i)
            {
                rankedArray[i, 0] = array[i];
            }

            return rankedArray;
        };
    }
}
