namespace TicTacToe.View
{
    /// <summary>
    /// Contains properties and static methods used for rendering menus.
    /// </summary>
    internal class Menu
    {
        /// <summary>
        /// Gets or sets the menu's header.
        /// </summary>
        /// <value>Header text; text to be displayed at the top of the menu.</value>
        protected string Header { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the menu's footer.
        /// </summary>
        /// <value>Footer text; text to be displayed at the bottom of the menu.</value>
        protected string Footer { get; set; } = string.Empty;

        /// <summary>
        /// Gets the a simple base menu footer used for both vanilla rendesr as well as renders using dynamic plugin components.
        /// </summary>
        /// <param name="isMenuCancellable">Whether or not the displayed menu is cancellable.</param>
        /// <returns>The menu's footer.</returns>
        internal static string GetBasicMenuFooter(bool isMenuCancellable = false)
        {
            return $@"
Navigate using up and down arrow keys.
Press Enter to select an option{(isMenuCancellable ? ", or Esc to return" : string.Empty)}.";
        }
    }
}
