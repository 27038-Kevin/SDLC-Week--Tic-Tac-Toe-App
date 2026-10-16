using TicTacToe.Enums;

namespace TicTacToe.Models.Data
{
    /// <summary>
    /// Represents a summary of a game that took place.
    /// </summary>
    /// <param name="Id">The game summary's unique identifier.</param>
    /// <param name="GameNo">The game number (also a unique identifier).</param>
    /// <param name="XPlayer">The name of the player playing X.</param>
    /// <param name="OPlayer">The name of the player playing O.</param>
    /// <param name="IsXHuman">Whether or not the player playing X is a human.</param>
    /// <param name="IsOHuman">Whether or not the player playing O is a human.</param>
    /// <param name="DateAndTime">The date and time on which the game was completed.</param>
    /// <param name="Moves">The moves played in the game in sequence.</param>
    /// <param name="GameOutcome">The final outcome of the game.</param>
    /// <param name="GameMode">The game mode.</param>
    internal record GameSummary() : IHasId, IEquatable<GameSummary>
    {
        public Guid Id { get; init; }
        public uint GameNo { get; init; }
        public string XPlayer { get; init; }
        public string OPlayer { get; init; }
        public bool IsXHuman { get; init; }
        public bool IsOHuman { get; init; }
        public DateTime DateAndTime { get; init; }
        public int[] Moves { get; init; }
        public GameOutcome GameOutcome { get; init; }
        public GameMode GameMode { get; init; }
    }
}
