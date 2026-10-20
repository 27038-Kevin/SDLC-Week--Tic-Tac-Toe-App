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
        public void RunWithStatelessCancellation(string menuHeader, string menuFooter, out TMenu? selectedChoice)
        {
            selectedChoice = default;
            try
            {
                selectedChoice = this.Run(menuHeader, menuFooter, true);
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
        /// <returns>Returns the user's selections from the menu.</returns>
        /// <exception cref="MenuCancelledException">Thrown when the user cancels the menu (given the menu is cancellable).</exception>
        /// <exception cref="ApplicationException">Thrown due to unforeseen system crashes or corruption in the internal state.</exception>
        public TMenu Run(string menuHeader, string menuFooter, bool isMenuCancellable)
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

            if (selector.ViewState == SelectorViewState.Selected)
            {
                return Enum.Parse<TMenu>(
                        CaseConverter.TitleToPascal(
                            this.MenuLines[selector.Selection]));
            }

            if (selector.ViewState == SelectorViewState.Cancelled)
            {
                throw new MenuCancelledException();
            }

            // Application should never be able to reach this line
            throw new ApplicationException("Corrupted state flow");
        }

        /// <summary>
        /// Renders the menu on-screen and returns the selected choices.
        /// Used for when the menu needs to persist, rather than returning a single selection.
        /// </summary>
        /// <param name="generateMenuHeader">The text to be displayed above the menu.</param>
        /// <param name="generateMenuFooter">The text to be displayed below the menu.</param>
        /// <param name="isMenuCancellable">Whether or not the user can cancel the menu.</param>
        /// <returns>Returns the user's selections from the menu.</returns>
        public IEnumerable<TMenu> RunPersistently(
                Func<string> generateMenuHeader,
                Func<string> generateMenuFooter,
                bool isMenuCancellable)
        {
            using var selector = new Selector(
                    this.MenuLines,
                    this.RenderMenu,
                    ConsoleUtils.ClearScreen,
                    isMenuCancellable);

            while (selector.ViewState != SelectorViewState.Cancelled)
            {
                this.Header = generateMenuHeader();
                this.Footer = generateMenuFooter();

                while (selector.ViewState == SelectorViewState.Active)
                {
                    // Dry refresh to render menu, in case it has been cleared
                    selector.Update(this.MenuLines, null);

                    ConsoleKeyInfo keyPress = ConsoleUtils.GetKeyPress();
                    selector.Update(this.MenuLines, keyPress);
                }

                if (selector.ViewState == SelectorViewState.Selected)
                {
                    yield return Enum.Parse<TMenu>(
                            CaseConverter.TitleToPascal(
                                this.MenuLines[selector.Selection]));

                    selector.Reset();
                }
            }
        }

        /// <summary>
        /// Display method for rendering the menu on-screen.
        /// </summary>
        /// <param name="menu">The menu to be rendered.</param>
        /// <param name="selectedOption">The currently selected menu option index.</param>
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
