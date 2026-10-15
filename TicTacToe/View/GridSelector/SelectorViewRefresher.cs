using TicTacToe.Enums;
using TicTacToe.Models.Utils;

namespace TicTacToe.View.GridSelector
{
    /// <summary>
    /// Refreshes the view with every update in state for the selector UI element.
    /// </summary>
    internal class SelectorViewRefresher : IDisposable
    {
        private readonly bool _isCancellable;

        private CartesianStateTracker _selected;

        private bool _disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectorViewRefresher"/> class.
        /// </summary>
        /// <param name="options">The matrix of options to be rendered in the selector view.</param>
        /// <param name="renderView">Method to render the selector view.</param>
        /// <param name="clearView">Method to clear the selector view.</param>
        /// <param name="isCancellable">Whether or not the selector menu can be returned from.</param>
        internal SelectorViewRefresher(
                string[,] options,
                Action<string[,], CartesianStateTracker> renderView,
                Action clearView,
                bool isCancellable)
        {
            this.Options = options;
            this.RenderView = renderView;
            this.ClearView = clearView;
            this._isCancellable = isCancellable;
        }

        /// <summary>
        /// Gets a single-cast action to render the view.
        /// </summary>
        /// <value>Method responsible for rendering the grid selector view.</value>
        public Action<string[,], CartesianStateTracker> RenderView { get; private set; }

        /// <summary>
        /// Gets a single-cast action to clear the view.
        /// </summary>
        /// <value>Method responsible for clearing or flushing the view render.</value>
        public Action ClearView { get; private set; }

        /// <summary>
        /// Gets the currently available choices.
        /// </summary>
        /// <value>A matrix representing the grid.</value>
        public string[,] Options { get; private set; }

        /// <summary>
        /// Disposes of the <see cref="SelectorViewRefresher"/> instance appropriately.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Refreshes the total state, including the state of attached external components.
        /// </summary>
        /// <param name="options">
        /// The available options to be rendered as a matrix on the search view.
        /// (Subject to change during dynamic refresh.)
        /// </param>
        /// <param name="lastKeyPress">The last key press made by the user.</param>
        /// <param name="stateTracker">Tracker that holds selection index and choice count.</param>
        /// <returns>The current selector view state.</returns>
        internal SelectorViewState RefreshState(
                string[,] options,
                ConsoleKeyInfo lastKeyPress,
                SelectorStateTracker stateTracker)
        {
            // Update the available choices and number of choices
            this.Options = options;
            stateTracker.RowCount = options.GetLength(0);
            stateTracker.ColumnCount = options.GetLength(1);

            // Wire the tracker's selected state to the refresher
            this._selected = stateTracker.Selected;

            return lastKeyPress.Key switch
            {
                ConsoleKey.UpArrow => stateTracker.KeyUp(this),

                ConsoleKey.DownArrow => stateTracker.KeyDown(this),

                ConsoleKey.LeftArrow => stateTracker.KeyLeft(this),

                ConsoleKey.RightArrow => stateTracker.KeyRight(this),

                ConsoleKey.Enter => this.Options.Length != 0
                                        ? stateTracker.Select(this)
                                        : this.RefreshView(),   // Do not select if the choice list is empty

                ConsoleKey.Escape => this._isCancellable
                                        ? stateTracker.Cancel(this)
                                        : this.RefreshView(),

                _ => stateTracker.UpdateExternalState(this, lastKeyPress),
            };
        }

        /// <summary>
        /// Refreshes the view.
        /// </summary>
        /// <returns>The current view state.</returns>
        internal SelectorViewState RefreshView()
        {
            this.ClearView?.Invoke();
            this.RenderView?.Invoke(this.Options, this._selected);
            return SelectorViewState.Active;
        }

        /// <summary>
        /// Selects an item from the view.
        /// </summary>
        /// <returns>The current view state.</returns>
        internal SelectorViewState SelectItem()
        {
            this.ClearView?.Invoke();
            return SelectorViewState.Selected;
        }

        /// <summary>
        /// Closes the view.
        /// </summary>
        /// <returns>The current view state.</returns>
        internal SelectorViewState CancelSelection()
        {
            this.ClearView?.Invoke();
            return SelectorViewState.Cancelled;
        }

        /// <summary>
        /// Update the selected index used during render.
        /// </summary>
        /// <param name="selected">The currently selected index.</param>
        internal void UpdateSelected(CartesianStateTracker selected)
        {
            this._selected = selected;
        }

        /// <summary>
        /// Disposes of managed resources.
        /// Resets delegates to allow garbage collection.
        /// </summary>
        /// <param name="disposing">Whether or not managed resources are currently being disposed.</param>
        private void Dispose(bool disposing)
        {
            if (this._disposed)
            {
                return;
            }

            if (disposing)
            {
                this.ClearView = default!;
                this.RenderView = default!;
            }

            this._disposed = true;
        }
    }
}
