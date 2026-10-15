namespace TicTacToe.Enums
{
    /// <summary>
    /// The current state of the selector view.
    /// </summary>
    public enum SelectorViewState
    {
        /// <summary>
        /// The selector view is currently active.
        /// </summary>
        Active,

        /// <summary>
        /// The user made a selection.
        /// </summary>
        Selected,

        /// <summary>
        /// The selection was cancelled.
        /// </summary>
        Cancelled,
    }
}
