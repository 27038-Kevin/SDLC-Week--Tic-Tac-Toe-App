using TicTacToe.Enums;
using TicTacToe.Models.Utils;

namespace TicTacToe.View.GridSelector
{
    /// <summary>
    /// Tracks the selector's current state.
    /// </summary>
    internal class SelectorStateTracker : IDisposable
    {
        private bool _disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectorStateTracker"/> class.
        /// </summary>
        /// <param name="gridLength">The maximum length of the grid.</param>
        /// <param name="gridHeight">The maximum height of the grid.</param>
        /// <param name="dynamicPluginListeners">Listeners of external dynamic plugin components.</param>
        public SelectorStateTracker(
                int gridLength,
                int gridHeight,
                params EventHandler<ConsoleKeyInfo>[] dynamicPluginListeners)
        {
            this.Selected = new CartesianStateTracker(gridLength, gridHeight);
            foreach (var listener in dynamicPluginListeners)
            {
                this.UpdateDynamicPlugins += listener;
            }
        }

        /// <summary>
        /// Event raised to update the states of other attachable dynamic plugin listeners.
        /// </summary>
        public event EventHandler<ConsoleKeyInfo>? UpdateDynamicPlugins;

        /// <summary>
        /// Gets or sets the number of rows in the grid.
        /// </summary>
        /// <value>The number of rows in the selector's grid.</value>
        public int RowCount { get; set; }

        /// <summary>
        /// Gets or sets the number of columns in the grid.
        /// </summary>
        /// <value>The number of columns in the selector's grid.</value>
        public int ColumnCount { get; set; }

        /// <summary>
        /// Gets the state of the current selection.
        /// </summary>
        /// <value>The Cartesian state tracker, which keeps track of the X and Y-axis coordinates of the selector.</value>
        public CartesianStateTracker Selected { get; private set; }

        /// <summary>
        /// Releases all resources used by the object.
        /// </summary>
        /// <remarks>
        /// Call Dispose when you are finished using the object to free unmanaged resources.
        /// After calling Dispose, do not use the object again.
        /// </remarks>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Moves the selection up by one if possible and refreshes the selector view.
        /// </summary>
        /// <param name="viewRefresher">Refreshes the selector view whenever there is a keypress.</param>
        /// <returns>The current state of the selector view.</returns>
        internal SelectorViewState KeyUp(SelectorViewRefresher viewRefresher)
        {
            if (this.Selected.Up())
            {
                viewRefresher.UpdateSelected(this.Selected);
            }

            return viewRefresher.RefreshView();
        }

        /// <summary>
        /// Moves the selection down by one if possible and refreshes the selector view.
        /// </summary>
        /// <param name="viewRefresher">Refreshes the selector view whenever there is a keypress.</param>
        /// <returns>The current state of the selector view.</returns>
        internal SelectorViewState KeyDown(SelectorViewRefresher viewRefresher)
        {
            if (this.Selected.Down())
            {
                viewRefresher.UpdateSelected(this.Selected);
            }

            return viewRefresher.RefreshView();
        }

        /// <summary>
        /// Moves the selection to the left by one if possible and refreshes the selector view.
        /// </summary>
        /// <param name="viewRefresher">Refreshes the selector view whenever there is a keypress.</param>
        /// <returns>The current state of the selector view.</returns>
        internal SelectorViewState KeyLeft(SelectorViewRefresher viewRefresher)
        {
            if (this.Selected.Left())
            {
                viewRefresher.UpdateSelected(this.Selected);
            }

            return viewRefresher.RefreshView();
        }

        /// <summary>
        /// Moves the selection to the right by one if possible and refreshes the selector view.
        /// </summary>
        /// <param name="viewRefresher">Refreshes the selector view whenever there is a keypress.</param>
        /// <returns>The current state of the selector view.</returns>
        internal SelectorViewState KeyRight(SelectorViewRefresher viewRefresher)
        {
            if (this.Selected.Right())
            {
                viewRefresher.UpdateSelected(this.Selected);
            }

            return viewRefresher.RefreshView();
        }

        /// <summary>
        /// Selects the currently selected option and updates the selector view.
        /// </summary>
        /// <param name="viewRefresher">Refreshes the selector view whenever there is a keypress.</param>
        /// <returns>The current state of the selector view.</returns>
        internal SelectorViewState Select(SelectorViewRefresher viewRefresher)
        {
            return viewRefresher.SelectItem();
        }

        /// <summary>
        /// Cancels the selection process and updates the selector view.
        /// </summary>
        /// <param name="viewRefresher">Refreshes the selector view whenever there is a keypress.</param>
        /// <returns>The current state of the selector view.</returns>
        internal SelectorViewState Cancel(SelectorViewRefresher viewRefresher)
        {
            return viewRefresher.CancelSelection();
        }

        /// <summary>
        /// Updates the external state of plugin components upon a keypress.
        /// </summary>
        /// <param name="viewRefresher">Refreshes the selector view whenever there is a keypress.</param>
        /// <param name="keyPress">The last keypress.</param>
        /// <returns>The current state of the selector view.</returns>
        internal SelectorViewState UpdateExternalState(SelectorViewRefresher viewRefresher, ConsoleKeyInfo keyPress)
        {
            this.UpdateDynamicPlugins?.Invoke(this, keyPress);
            return viewRefresher.RefreshView();
        }

        /// <summary>
        /// Removes all dynamic plugins subscribed to the Update event, freeing up resources to avoid memory leaks.
        /// </summary>
        /// <param name="disposing">true if currently resetting Update event; false otherwise.</param>
        internal void Dispose(bool disposing)
        {
            if (this._disposed)
            {
                return;
            }

            if (disposing)
            {
                this.UpdateDynamicPlugins = default;
            }

            this._disposed = true;
        }
    }
}
