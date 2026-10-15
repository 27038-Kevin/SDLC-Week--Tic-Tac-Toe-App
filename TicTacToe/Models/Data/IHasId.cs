namespace TicTacToe.Models.Data
{
    /// <summary>
    /// Defines a contract for a model that has a unique identifier.
    /// </summary>
    public interface IHasId
    {
        /// <summary>
        /// Gets the GUID of the model.
        /// </summary>
        /// <value>The unique identifier of the item.</value>
        Guid Id { get; init; }
    }
}
