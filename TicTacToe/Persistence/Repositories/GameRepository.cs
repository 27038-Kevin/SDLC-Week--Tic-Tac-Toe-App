using System.ComponentModel;
using TicTacToe.Enums;
using TicTacToe.Models.Data;
using TicTacToe.Models.Utils;
using TicTacToe.Persistence.Serializers;

namespace TicTacToe.Persistence.Repositories
{
    /// <summary>
    /// Abstracts underlying file operations to aggregate and manage <see cref="GameSummary"/> instances.
    /// </summary>
    internal sealed class GameRepository : FileRepository<GameSummary>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GameRepository"/> class.
        /// </summary>
        /// <param name="fileDirectory">The directory in which the games data file is present in.</param>
        /// <param name="fileName">The games data file name.</param>
        /// <param name="serializer">The serializer used for serializing game summaries to strings and vice versa.</param>
        public GameRepository(
                string fileDirectory,
                string fileName,
                ISerializer<GameSummary> serializer)
            : base(fileDirectory, fileName, serializer)
        {
        }

        /// <summary>
        /// Filters the list of game summaries based on specific conditions.
        /// </summary>
        /// <param name="filter">The filter struct containing data to generate conditions to filter by.</param>
        /// <returns>The filtered list of game summaries.</returns>
        internal IEnumerable<GameSummary> Filter(GameSummaryFilter filter)
        {
            var summaries = this.GetAll();

            if (filter.DateRange is not null)
            {
                summaries = summaries.Where(
                    game => filter.DateRange.Value
                                .Contains(game.DateAndTime));
            }

            if (filter.Mode is not null)
            {
                summaries = summaries.Where(game => game.GameMode == filter.Mode);
            }

            if (filter.Outcome is not null)
            {
                summaries = filter.Outcome switch
                {
                    PlayerOrientedGameOutcome.Win => summaries.Where(
                            game => game.GameMode != GameMode.Manual &&
                                ((game.IsXHuman && game.GameOutcome == GameOutcome.XWon)
                                || (game.IsOHuman && game.GameOutcome == GameOutcome.OWon))),

                    PlayerOrientedGameOutcome.Loss => summaries.Where(
                            game => game.GameMode != GameMode.Manual &&
                                ((game.IsXHuman && game.GameOutcome == GameOutcome.OWon)
                                || (game.IsOHuman && game.GameOutcome == GameOutcome.XWon))),

                    PlayerOrientedGameOutcome.Tie => summaries.Where(game => game.GameOutcome == GameOutcome.Tie),

                    PlayerOrientedGameOutcome.Quit => summaries.Where(game => game.GameOutcome == GameOutcome.Quit),

                    var _ => throw new InvalidEnumArgumentException(nameof(filter.Outcome)),
                };
            }

            if (filter.LetterPlayed is not null)
            {
                summaries = summaries.Where(game => game.GameMode != GameMode.Manual);
                summaries = filter.LetterPlayed switch
                {
                    Letter.X => summaries.Where(game => game.IsXHuman),
                    Letter.O => summaries.Where(game => game.IsOHuman),
                    var _ => throw new InvalidEnumArgumentException(nameof(filter.LetterPlayed)),
                };
            }

            return summaries;
        }

        /// <summary>
        /// Finds a game summary for a game based on the game number.
        /// </summary>
        /// <param name="gameNumber">The game number. A candidate key rivalling GUID for better user readability.</param>
        /// <returns>The game summary for the specified game number, if found.</returns>
        /// <exception cref="KeyNotFoundException">Thrown if no such game exists.</exception>
        internal GameSummary GetByGameNumber(uint gameNumber)
        {
            return this.GetAll().FirstOrDefault(game => game.GameNo == gameNumber)
                ?? throw new KeyNotFoundException($"Game with game no. {gameNumber} does not exist");
        }
    }
}
