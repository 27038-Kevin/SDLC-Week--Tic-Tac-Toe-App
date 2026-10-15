using TicTacToe.Enums;

namespace TicTacToe.Models.Data
{
    /// <summary>
    /// Represents the statistics for the user in a particular gamemode.
    /// </summary>
    internal readonly struct GameStats
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GameStats"/> struct.
        /// </summary>
        /// <param name="mode">The mode the stats are selected for.</param>
        /// <param name="totalGames">The total number of games in that specific mode.</param>
        /// <param name="gamesPlayedAsX">The number of games the user played as X.</param>
        /// <param name="gamesPlayedAsO">The number of games the user played as O.</param>
        /// <param name="gamesWon">The number of games won by the user.</param>
        /// <param name="gamesLost">The number of games lost by the user.</param>
        /// <param name="gamesTied">The number of games tied.</param>
        public GameStats(
                GameMode mode,
                int totalGames,
                int gamesPlayedAsX,
                int gamesPlayedAsO,
                int gamesWon,
                int gamesLost,
                int gamesTied)
        {
            this.Mode = mode;
            this.TotalGames = totalGames;
            this.GamesPlayedAsX = gamesPlayedAsX;
            this.GamesPlayedAsO = gamesPlayedAsO;
            this.GamesWon = gamesWon;
            this.GamesLost = gamesLost;
            this.GamesTied = gamesTied;
        }

        /// <summary>
        /// Gets the game mode.
        /// </summary>
        /// <value>The game mode (see values at <see cref="GameMode"/>.</value>
        public readonly GameMode Mode { get; init; }

        /// <summary>
        /// Gets the total number of games played in this mode.
        /// </summary>
        /// <value>The total number of games played in this mode.</value>
        public readonly int TotalGames { get; init; }

        /// <summary>
        /// Gets the total number of games the user played as X.
        /// </summary>
        /// <value>The total number of games played as X.</value>
        public readonly int GamesPlayedAsX { get; init; }

        /// <summary>
        /// Gets the total number of games the user played as O.
        /// </summary>
        /// <value>The total number of games played as O.</value>
        public readonly int GamesPlayedAsO { get; init; }

        /// <summary>
        /// Gets the total number of games won in this mode.
        /// </summary>
        /// <value>The number of wins in this gamemode.</value>
        public readonly int GamesWon { get; init; }

        /// <summary>
        /// Gets the total number of games lost in this mode.
        /// </summary>
        /// <value>The number of losses in this gamemode.</value>
        public readonly int GamesLost { get; init; }

        /// <summary>
        /// Gets the total number of games tied in this mode.
        /// </summary>
        /// <value>The number of ties in this gamemode.</value>
        public readonly int GamesTied { get; init; }

        /// <summary>
        /// Gets the winrate percentage formatted as a string.
        /// </summary>
        /// <value>
        /// NaN if no games are played, 0 if no games are won;
        /// otherwise, gets the winrate percentage formatted to two decimal places.
        /// </value>
        public readonly string WinRate => $"{(double)this.GamesWon / this.TotalGames * 100:F2}%";

        /// <summary>
        /// Gets the win-loss ratio formatted as a string.
        /// </summary>
        /// <value>
        /// NaN if no games are played;
        /// Infinity if no games were lost;
        /// 0 if no games were won;
        /// otherwise, gets the win-loss ratio formatted to three decimal places.
        /// </value>
        public readonly string WinLossRatio => $"{(double)this.GamesWon / this.GamesLost:F3}";
    }
}
