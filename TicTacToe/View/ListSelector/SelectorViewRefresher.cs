using TicTacToe.Enums;

namespace TicTacToe.View.ListSelector
{
    /// <summary>
    /// Refreshes the view with every update in state for the selector UI element.
    /// </summary>
    internal class SelectorViewRefresher : IDisposable
    {
        /// <summary>
        /// Stores the currently selected choice index used when rendering.
        /// </summary>
        private int _selected = 0;

        /// <summary>
        /// Stores whether the menu can be cancelled with the Esc key.
        /// </summary>
        private bool _isCancellable;

        private bool _disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectorViewRefresher"/> class.
        /// </summary>
        /// <param name="choices">The list of choices to be rendered in the selector view.</param>
        /// <param name="renderView">Method to render the selector view.</param>
        /// <param name="clearView">Method to clear the selector view.</param>
        /// <param name="isCancellable">Whether or not the selector menu can be returned from.</param>
        internal SelectorViewRefresher(
                IList<string> choices,
                Action<IList<string>, int> renderView,
                Action clearView,
                bool isCancellable)
        {
            this.Choices = choices;
            this.RenderView = renderView;
            this.ClearView = clearView;
            this._isCancellable = isCancellable;
        }

        /// <summary>
        /// Gets a single-cast action to render the view.
        /// </summary>
        /// <value>Delegate which renders the view when invoked.</value>
        public Action<IList<string>, int> RenderView { get; private set; }

        /// <summary>
        /// Gets a single-cast action to clear the view.
        /// </summary>
        /// <value>Delegate which clears the screen when invoked.</value>
        public Action ClearView { get; private set; }

        /// <summary>
        /// Gets the currently available choices.
        /// </summary>
        /// <value>The choices currently present in the selector view.</value>
        public IList<string> Choices { get; private set; }

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
        /// Refreshes the total state, including the state of attached external components.
        /// </summary>
        /// <param name="choices">The available choices to be rendered on the search view. (Subject to change during dynamic live search.)</param>
        /// <param name="lastKeyPress">The last key press made by the user.</param>
        /// <param name="stateTracker">Tracker that holds selection index and choice count.</param>
        /// <returns>The current selector view state.</returns>
        internal SelectorViewState RefreshState(
                IList<string> choices,
                ConsoleKeyInfo? lastKeyPress,
                SelectorStateTracker stateTracker)
        {
            // Update the available choices and number of choices
            this.Choices = choices ?? Array.Empty<string>();
            stateTracker.ChoicesCount = choices?.Count ?? 0;

            // Wire the tracker's selected state to the refresher
            this._selected = stateTracker.Selected;

            return lastKeyPress?.Key switch
            {
                ConsoleKey.UpArrow => stateTracker.KeyUp(this),

                ConsoleKey.DownArrow => stateTracker.KeyDown(this),

                ConsoleKey.Enter => (stateTracker.ChoicesCount != 0)
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
            this.RenderView?.Invoke(this.Choices, this._selected);
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
        internal void UpdateSelected(int selected)
        {
            this._selected = selected;
        }

        /// <summary>
        /// Disposes of managed memory resources;
        /// more specifically, resets the two singlecast delegates used for view refresh.
        /// </summary>
        /// <param name="disposing">true if the delegates are currently being disposed; false otherwise.</param>
        private void Dispose(bool disposing)
        {
            if (this._disposed)
            {
                return;
            }

            if (disposing)
            {
                this.RenderView = default!;
                this.ClearView = default!;
            }

            this._disposed = true;
        }
    }
}
