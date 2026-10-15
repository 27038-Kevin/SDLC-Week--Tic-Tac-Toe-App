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
    internal record GameSummary(
                    Guid Id,
                    uint GameNo,
                    string XPlayer,
                    string OPlayer,
                    bool IsXHuman,
                    bool IsOHuman,
                    DateTime DateAndTime,
                    int[] Moves,
                    GameOutcome GameOutcome,
                    GameMode GameMode)
            : IHasId;
}
