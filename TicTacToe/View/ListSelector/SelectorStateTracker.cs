using TicTacToe.Enums;

namespace TicTacToe.View.ListSelector
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
        /// <param name="choicesCount">The number of results to choose from.</param>
        /// <param name="dynamicPluginListeners">Listeners of external dynamic plugin components.</param>
        public SelectorStateTracker(int choicesCount, params EventHandler<ConsoleKeyInfo?>[] dynamicPluginListeners)
        {
            this.ChoicesCount = choicesCount;
            this.Selected = 0;

            foreach (var listener in dynamicPluginListeners)
            {
                this.UpdateDynamicPlugins += listener;
            }
        }

        /// <summary>
        /// Event raised to update the states of other attachable dynamic plugin listeners.
        /// </summary>
        public event EventHandler<ConsoleKeyInfo?>? UpdateDynamicPlugins;

        /// <summary>
        /// Gets or sets the number of results that can be chosen from the selector view.
        /// </summary>
        /// <value>The number of choices present in the selector view.</value>
        public int ChoicesCount { get; set; }

        /// <summary>
        /// Gets the currently selected result in the selector view.
        /// </summary>
        /// <value>The index of the currently selected choice.</value>
        public int Selected { get; private set; }

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
            if (this.Selected > 0)
            {
                --this.Selected;
            }

            viewRefresher.UpdateSelected(this.Selected);
            return viewRefresher.RefreshView();
        }

        /// <summary>
        /// Moves the selection down by one if possible and refreshes the selector view.
        /// </summary>
        /// <param name="viewRefresher">Refreshes the selector view whenever there is a keypress.</param>
        /// <returns>The current state of the selector view.</returns>
        internal SelectorViewState KeyDown(SelectorViewRefresher viewRefresher)
        {
            if (this.Selected < this.ChoicesCount - 1)
            {
                ++this.Selected;
            }

            viewRefresher.UpdateSelected(this.Selected);
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
        internal SelectorViewState UpdateExternalState(SelectorViewRefresher viewRefresher, ConsoleKeyInfo? keyPress)
        {
            this.UpdateDynamicPlugins?.Invoke(this, keyPress);
            return viewRefresher.RefreshView();
        }

        /// <summary>
        /// Resets the internal state of the selector.
        /// </summary>
        internal void Reset()
        {
            this.Selected = 0;
        }

        /// <summary>
        /// Removes all dynamic plugins subscribed to the Update event, freeing up resources to avoid memory leaks.
        /// </summary>
        /// <param name="disposing">true if currently resetting Update event; false otherwise.</param>
        private void Dispose(bool disposing)
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
