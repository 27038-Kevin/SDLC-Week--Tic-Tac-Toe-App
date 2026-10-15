using TicTacToe.Enums;
using TicTacToe.Helpers;
using TicTacToe.Models.Utils;

namespace TicTacToe.View.GridSelector
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
        /// <param name="grid">The initial matrix of options present in the selector view.</param>
        /// <param name="renderView">The action required to render the selector view.</param>
        /// <param name="clearView">The action required to clear the selector view.</param>
        /// <param name="isCancellable">Whether or not the selector menu can be returned from.</param>
        /// <param name="externalKeyboardListeners">
        /// Listeners of other dynamic view components that listen for other non-navigation keypresses.
        /// Enables versatility by utilizing a "plug n' play" API design.
        /// </param>
        public Selector(
                string[,] grid,
                Action<string[,], CartesianStateTracker> renderView,
                Action clearView,
                bool isCancellable,
                params EventHandler<ConsoleKeyInfo>[] externalKeyboardListeners)
        {
            this._viewRefresher = new SelectorViewRefresher(grid, renderView, clearView, isCancellable);
            this._stateTracker = new SelectorStateTracker(
                                        grid.GetLength(1),
                                        grid.GetLength(0),
                                        externalKeyboardListeners);

            this._viewRefresher.RefreshView();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Selector"/> class.
        /// Adapts a one-dimensional array to use with the selector instance.
        /// </summary>
        /// <param name="list">The list to be adapted and used within the selector.</param>
        /// <param name="renderView">The action required to render the selector view.</param>
        /// <param name="clearView">The action required to clear the selector view.</param>
        /// <param name="isCancellable">Whether or not the selector menu can be returned from.</param>
        /// <param name="externalKeyboardListeners">
        /// Listeners of other dynamic view components that listen for other non-navigation keypresses.
        /// Enables versatility by utilizing a "plug n' play" API design.
        /// </param>
        public Selector(
                string[] list,
                Action<string[,], CartesianStateTracker> renderView,
                Action clearView,
                bool isCancellable,
                params EventHandler<ConsoleKeyInfo>[] externalKeyboardListeners)

            : this(
                ParamAdapter.ToRankedArray(list),
                renderView,
                clearView,
                isCancellable,
                externalKeyboardListeners)
        {
        }

        /// <summary>
        /// Gets the current view state.
        /// </summary>
        /// <value>The current status of the selector's view renderer.</value>
        public SelectorViewState ViewState { get; private set; } = SelectorViewState.Active;

        /// <summary>
        /// Gets the current selection.
        /// </summary>
        /// <value>The X-axis and Y-axis coordinates of the current selection.</value>
        public CartesianStateTracker Selection => this._stateTracker.Selected;

        /// <summary>
        /// Updates the selector's state and view.
        /// </summary>
        /// <param name="grid">The new grid to present in the view.</param>
        /// <param name="lastKeyPress">The last key pressed by the user.</param>
        public void UpdateSelector(string[,] grid, ConsoleKeyInfo lastKeyPress)
        {
            this.ViewState = this._viewRefresher.RefreshState(grid, lastKeyPress, this._stateTracker);
        }

        /// <summary>
        /// Updates the selector's state and view.
        /// Overload method to adapt the selector to single-dimensional arrays as well.
        /// </summary>
        /// <param name="list">The new list to present in the view.</param>
        /// <param name="lastKeyPress">The last key pressed by the user.</param>
        public void UpdateSelector(string[] list, ConsoleKeyInfo lastKeyPress)
        {
            this.ViewState = this._viewRefresher.RefreshState(
                    ParamAdapter.ToRankedArray(list),
                    lastKeyPress,
                    this._stateTracker);
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
