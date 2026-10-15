namespace TicTacToe.Models.Utils
{
    /// <summary>
    /// Represents a date range; contains two <see cref="DateTime"/> structs representing an inclusive range.
    /// </summary>
    public readonly struct DateTimeRange
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DateTimeRange"/> struct.
        /// </summary>
        /// <param name="startingDateTime">The inclusive starting date of the range.</param>
        /// <param name="endingDateTime">The inclusive ending date of the range.</param>
        /// <exception cref="ArgumentException">Thrown if the starting date occurs after the ending date.</exception>
        public DateTimeRange(DateTime startingDateTime, DateTime endingDateTime)
        {
            if (startingDateTime > endingDateTime)
            {
                throw new ArgumentException(
                        "Starting datetime must come before ending datetime",
                        nameof(startingDateTime));
            }

            this.Start = startingDateTime;
            this.End = endingDateTime;
        }

        /// <summary>
        /// Gets the starting date.
        /// </summary>
        /// <value>The starting date.</value>
        public DateTime Start { get; init; }

        /// <summary>
        /// Gets the ending date.
        /// </summary>
        /// <value>The ending date.</value>
        public DateTime End { get; init; }

        /// <summary>
        /// Discerns whether a given DateTime is present in the represented range.
        /// </summary>
        /// <param name="date">The date to confirm.</param>
        /// <returns>true if the date lies in the range; false otherwise.</returns>
        public readonly bool Contains(DateTime date)
        {
            return date >= this.Start && date <= this.End;
        }
    }
}
