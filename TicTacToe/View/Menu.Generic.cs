using TicTacToe.Enums;
using TicTacToe.Exceptions;
using TicTacToe.Helpers;
using TicTacToe.View.ListSelector;

namespace TicTacToe.View
{
    /// <summary>
    /// Generates menus to be rendered in the console.
    /// </summary>
    /// <typeparam name="TMenu">The type of menu to be handled.</typeparam>
    internal class Menu<TMenu> : Menu
            where TMenu : struct, Enum
    {
        /// <summary>
        /// Gets the menu's lines as individual strings.
        /// </summary>
        /// <value>The lines present in the menu.</value>
        internal string[] MenuLines { get; } = Enum.GetNames<TMenu>()
                                                .Select(CaseConverter.PascalToTitle)
                                                .ToArray();

        /// <summary>
        /// Renders the menu and attempts to take a selection from the user,
        /// while allowing for easy stateless cancellation if the user wants to exit.
        /// </summary>
        /// <param name="menuHeader">The menu's header.</param>
        /// <param name="menuFooter">The menu's footer.</param>
        /// <param name="selectedChoice">The choice selected by the user, if any.</param>
        public void RenderWithStatelessCancellation(string menuHeader, string menuFooter, out TMenu? selectedChoice)
        {
            selectedChoice = default;
            try
            {
                selectedChoice = this.Render(menuHeader, menuFooter, true);
            }
            catch (MenuCancelledException)
            {
                return; // Stateless return, enabling rapid switching to previous menu
            }
        }

        /// <summary>
        /// Renders the menu on-screen an returns the selected choice.
        /// </summary>
        /// <param name="menuHeader">The text to be displayed above the menu.</param>
        /// <param name="menuFooter">The text to be displayed below the menu.</param>
        /// <param name="isMenuCancellable">Whether or not the user can cancel the menu.</param>
        /// <returns>Returns the user's selection from the menu.</returns>
        /// <exception cref="MenuCancelledException">Thrown when the user cancels the menu (given the menu is cancellable).</exception>
        /// <exception cref="ApplicationException">Thrown due to unforeseen system crashes or corruption in the internal state.</exception>
        public TMenu Render(string menuHeader, string menuFooter, bool isMenuCancellable)
        {
            this.Header = menuHeader;
            this.Footer = menuFooter;

            using var selector = new Selector(
                    this.MenuLines,
                    this.RenderMenu,
                    ConsoleUtils.ClearScreen,
                    isMenuCancellable);

            while (selector.ViewState == SelectorViewState.Active)
            {
                ConsoleKeyInfo keyPress = ConsoleUtils.GetKeyPress();
                selector.Update(this.MenuLines, keyPress);
            }

            return selector.ViewState switch
            {
                SelectorViewState.Selected => Enum.Parse<TMenu>(
                                                CaseConverter.TitleToPascal(
                                                    this.MenuLines[selector.Selection])),

                SelectorViewState.Cancelled => throw new MenuCancelledException(),

                _ => throw new ApplicationException("Unexpected state corruption"), // Can only be triggered during debug mode
            };
        }

        private void RenderMenu(IList<string> menu, int selectedOption)
        {
            Console.WriteLine(this.Header);
            Console.WriteLine();

            for (int i = 0; i < menu.Count; ++i)
            {
                if (i == selectedOption)
                {
                    Console.ForegroundColor = ConsoleColor.DarkBlue;
                    Console.BackgroundColor = ConsoleColor.White;
                }

                Console.WriteLine(menu[i]);

                if (i == selectedOption)
                {
                    Console.ResetColor();
                }
            }

            Console.WriteLine(this.Footer);
        }
    }
}
