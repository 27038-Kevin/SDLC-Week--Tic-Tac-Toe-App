using TicTacToe.Enums;

namespace TicTacToe.View.ListSelector
{
    /// <summary>
    /// Element for navigating menus and search views using arrow keys.
    /// </summary>
    public class Selector : IDisposable
    {
        private readonly SelectorStateTracker _stateTracker;
        private readonly SelectorViewRefresher _viewRefresher;

        private bool _disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="Selector"/> class.
        /// </summary>
        /// <param name="choices">The initial list of choices present in the selector view.</param>
        /// <param name="renderView">The action required to render the selector view.</param>
        /// <param name="clearView">The action required to clear the selector view.</param>
        /// <param name="isCancellable">Whether or not the selector menu can be returned from.</param>
        /// <param name="externalKeyboardListeners">
        /// Listeners of other dynamic view components that listen for other non-navigation keypresses.
        /// Enables versatility by utilizing a "plug n' play" API design.
        /// </param>
        public Selector(
                IList<string> choices,
                Action<IList<string>, int> renderView,
                Action clearView,
                bool isCancellable,
                params EventHandler<ConsoleKeyInfo?>[] externalKeyboardListeners)
        {
            this._viewRefresher = new SelectorViewRefresher(choices, renderView, clearView, isCancellable);
            this._stateTracker = new SelectorStateTracker(choices.Count, externalKeyboardListeners);

            this._viewRefresher.RefreshView();
        }

        /// <summary>
        /// Gets the current view state.
        /// </summary>
        /// <value>The current status of the selector's renderer.</value>
        public SelectorViewState ViewState { get; private set; } = SelectorViewState.Active;

        /// <summary>
        /// Gets the current selection.
        /// </summary>
        /// <value>The index of the current selection from a collection.</value>
        public int Selection => this._stateTracker.Selected;

        /// <summary>
        /// Updates the selector's state and view.
        /// </summary>
        /// <param name="choices">The new list of choices present in the view.</param>
        /// <param name="lastKeyPress">The last key pressed by the user.</param>
        public void Update(IList<string> choices, ConsoleKeyInfo? lastKeyPress)
        {
            this.ViewState = this._viewRefresher.RefreshState(choices, lastKeyPress, this._stateTracker);
        }

        /// <summary>
        /// Releases all resources used by the object.
        /// </summary>
        /// <remarks>
        /// Call Dispose() when the object is no longer needed
        /// to free unmanaged resources and suppress finalization.
        /// </remarks>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases resources used by state tracker and view refresher components.
        /// </summary>
        /// <param name="disposing">
        /// true to release both managed and unmanaged resources;
        /// false to release only unmanaged resources (none).
        /// </param>
        private void Dispose(bool disposing)
        {
            if (this._disposed)
            {
                return;
            }

            if (disposing)
            {
                this._stateTracker.Dispose();
                this._viewRefresher.Dispose();
            }

            this._disposed = true;
        }
    }
}
